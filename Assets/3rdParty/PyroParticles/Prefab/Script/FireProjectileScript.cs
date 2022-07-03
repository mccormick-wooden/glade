using UnityEngine;
using System.Collections;
using System.Linq;
using Beacons;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace DigitalRuby.PyroParticles
{
    /// <summary>
    /// Handle collision of a fire projectile
    /// </summary>
    /// <param name="script">Script</param>
    /// <param name="pos">Position</param>
    public delegate void FireProjectileCollisionDelegate(FireProjectileScript script, Vector3 pos);

    /// <summary>
    /// This script handles a projectile such as a fire ball
    /// </summary>
    public class FireProjectileScript : FireBaseScript, ICollisionHandler
    {
        [Tooltip("The collider object to use for collision and physics.")]
        public GameObject ProjectileColliderObject;

        [Tooltip("The sound to play upon collision.")]
        public AudioSource ProjectileCollisionSound;

        [Tooltip("The particle system to play upon collision.")]
        public ParticleSystem ProjectileExplosionParticleSystem;

        [Tooltip("The radius of the explosion upon collision.")]
        public float ProjectileExplosionRadius = 50.0f;

        [Tooltip("The force of the explosion upon collision.")]
        public float ProjectileExplosionForce = 50.0f;

        [Tooltip("An optional delay before the collider is sent off, in case the effect has a pre fire animation.")]
        public float ProjectileColliderDelay = 0.0f;

        [Tooltip("The speed of the collider.")]
        public float ProjectileColliderSpeed = 450.0f;

        [Tooltip(
            "The direction that the collider will go. For example, flame strike goes down, and fireball goes forward.")]
        public Vector3 ProjectileDirection = Vector3.forward;

        [Tooltip("What layers the collider can collide with.")]
        public LayerMask ProjectileCollisionLayers = Physics.AllLayers;

        [Tooltip("Particle systems to destroy upon collision.")]
        public ParticleSystem[] ProjectileDestroyParticleSystemsOnCollision;

        [HideInInspector] public FireProjectileCollisionDelegate CollisionDelegate;

        #region Glade Fields

        [Header("Glade Fields")]
        /*
         * This maps to the index of the layer we want the fireball to collide with. Go to the Terrain inspector and
         * check the ordering of the layers to decide which layer we want to collide with.
         */
        public int desiredCollisionTerrainLayerIndex;

        [SerializeField] private float minDistanceBetweenCrashedBeacons;

        private bool _hasCollided;
        public Quaternion firedRotation; // Accessed by the base fire script

        #endregion

        private IEnumerator SendCollisionAfterDelay()
        {
            yield return new WaitForSeconds(ProjectileColliderDelay);

            // firedRotation is thrown in the mix here to alter the projectile's trajectory
            Vector3 dir = firedRotation * ProjectileDirection * ProjectileColliderSpeed;
            dir = ProjectileColliderObject.transform.rotation * dir;
            ProjectileColliderObject.GetComponent<Rigidbody>().velocity = dir;
        }

        #region Falling Beacon Functions

        private bool IsLandingSpotCloseToBeacon(Vector3 landingSpotPosition)
        {
            return FindObjectsOfType<CrashedBeacon>()
                .Select(beacon => Vector3.Distance(landingSpotPosition, beacon.transform.position))
                .Any(distance =>
                {
                    Debug.Log(distance);
                    return distance <= minDistanceBetweenCrashedBeacons;
                });
        }

        private Quaternion FindAppropriateProjectileRotation()
        {
            /* Cache some repeatedly used and expensive to access fields */
            var activeTerrain = Terrain.activeTerrain;
            Utility.LogErrorIfNull(activeTerrain, "activeTerrain", "No active terrain, fire projectile will not fire!");
            var terrainData = activeTerrain.terrainData;
            var terrainAlphaMap =
                terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);

            float maxRayCastDistance = terrainData.size.x;

            var attempts = 0;
            while (true)
            {
                /* Loop: Repeatedly and stochastically attempt to hit the desired Terrain layer
                 * 
                 * Arbitrary stopgap here because the Editor/game will freeze if this is really going forever,
                 * and it usually doesn't make it past 10 iterations
                 */
                if (++attempts > 100)
                {
                    Debug.Log(
                        "Unable to find a suitable beacon landing spot after 100 attempts, firing straight down.");
                    return Quaternion.identity;
                }

                ;

                // Pick a random direction to fly off into
                var proposedRotation = Quaternion.Euler(Random.Range(-45, 45), Random.Range(-45, 45), 0f);

                // Compose the random direction with the base direction and rotation of the projectile
                Vector3 proposedTrajectory = ProjectileColliderObject.transform.rotation *
                                             (proposedRotation * ProjectileDirection);

                Physics.Raycast(transform.position, proposedTrajectory, out RaycastHit hit, maxRayCastDistance);
                //Debug.DrawRay(transform.position, proposedTrajectory * maxRayCastDistance, Color.red, 10f);

                /*
                 * Conditions to keep searching for a landing spot:
                 * - We did not hit anything
                 * - We hit a TerrainBridge tagged object
                 * - We are too close to an existing beacon
                 */
                if (hit.collider == null || hit.collider.gameObject.CompareTag("TerrainBridge") ||
                    IsLandingSpotCloseToBeacon(hit.point))
                {
                    continue;
                }

                if (hit.collider.TryGetComponent<Terrain>(out Terrain terrain))
                {
                    if (terrain.GetInstanceID() != activeTerrain.GetInstanceID())
                    {
                        /* We're treating there being a single terrain in the scene as invariant, but this may change in the future */
                        Debug.LogError(
                            "Collided with a terrain that is not the active terrain, this should not happen!");
                        return proposedRotation;
                    }

                    var hitPoint = hit.point;

                    /* The Terrain may be at a transformed position in the scene */
                    Vector3 terrainRelativePosition = hitPoint - terrain.transform.position;

                    /* Convert the RayCast hit position - now relative to the Terrain's position - to a position on the Alpha Map */
                    // Alpha Map: aka Splat Map, aka Texture map, etc
                    var alphaMapPosition = new Vector3
                    (
                        (terrainRelativePosition.x / terrainData.size.x) * terrainData.alphamapWidth,
                        0,
                        (terrainRelativePosition.z / terrainData.size.z) * terrainData.alphamapHeight
                    );

                    var alphaMapX = Mathf.RoundToInt(alphaMapPosition.x);
                    var alphaMapZ = Mathf.RoundToInt(alphaMapPosition.z);

                    int mostDominantTextureIndex = 0;
                    float greatestTextureWeight = float.MinValue;

                    /* Loop over the available textures and decide which texture is most dominant at the hit point */
                    int textureCount = terrainAlphaMap.GetLength(2);
                    for (int textureIndex = 0; textureIndex < textureCount; textureIndex++)
                    {
                        float textureWeight = terrainAlphaMap[alphaMapZ, alphaMapX, textureIndex];

                        if (textureWeight > greatestTextureWeight)
                        {
                            greatestTextureWeight = textureWeight;
                            mostDominantTextureIndex = textureIndex;
                        }
                    }

                    if (mostDominantTextureIndex == desiredCollisionTerrainLayerIndex)
                    {
                        // Debug.Log("Found a valid proposed rotation after " + attempts + " attempts");
                        return proposedRotation;
                    }
                }
            }
        }

        #endregion

        protected override void Awake()
        {
            base.Awake();

            /* This maps to the grass layer currently */
            desiredCollisionTerrainLayerIndex = 0;

            firedRotation = FindAppropriateProjectileRotation();
        }

        protected override void Start()
        {
            base.Start();

            StartCoroutine(SendCollisionAfterDelay());
        }

        public void HandleCollision(GameObject obj, Collision c)
        {
            if (_hasCollided)
            {
                // already collided, don't do anything
                return;
            }

            if ((ProjectileCollisionLayers.value & 1 << c.gameObject.layer) == 0)
            {
                return;
            }

            // stop the projectile
            _hasCollided = true;
            Stop();

            // destroy particle systems after a slight delay
            if (ProjectileDestroyParticleSystemsOnCollision != null)
            {
                foreach (ParticleSystem p in ProjectileDestroyParticleSystemsOnCollision)
                {
                    GameObject.Destroy(p, 0.1f);
                }
            }

            // play collision sound
            if (ProjectileCollisionSound != null)
            {
                ProjectileCollisionSound.Play();
            }

            // if we have contacts, play the collision particle system and call the delegate
            if (c.contacts.Length != 0)
            {
                ProjectileExplosionParticleSystem.transform.position = c.contacts[0].point;
                ProjectileExplosionParticleSystem.Play();
                FireBaseScript.CreateExplosion(c.contacts[0].point, ProjectileExplosionRadius,
                    ProjectileExplosionForce);
                if (CollisionDelegate != null)
                {
                    CollisionDelegate(this, c.contacts[0].point);
                }
            }
        }
    }
}
