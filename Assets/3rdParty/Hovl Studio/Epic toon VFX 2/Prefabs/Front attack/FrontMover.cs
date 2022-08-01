using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Abstract;
using UnityEngine;

public class FrontMover : SpecialEffectWeapon
{
    public Transform pivot;
    public ParticleSystem effect;
    public float speed = 15f;
    public float drug = 1f;
    public float repeatingTime = 1f;

    private float startSpeed = 0f;

    protected override void Start()
    {
        base.Start();
        baseAttackDamage = 10f;
        currentAttackDamage = baseAttackDamage;
        InvokeRepeating(nameof(StartAgain), 0f, repeatingTime);
        effect.Play();
        startSpeed = speed;
    }

    private void StartAgain()
    {
        startSpeed = speed;
        transform.position = pivot.position;
    }

    private void Update()
    {
        startSpeed = startSpeed * drug;
        Vector3 targetPosition = transform.position + transform.forward * (startSpeed * Time.deltaTime);
        float terrainModifiedHeight = Terrain.activeTerrain.SampleHeight(transform.position);

        // Determine slope at world position
        Terrain terrain = Terrain.activeTerrain;
        Vector3 terrainPos3 = targetPosition - terrain.transform.position;
        Vector2 terrainPos = new Vector2(terrainPos3.x, terrainPos3.z);
        TerrainData terrainData = terrain.terrainData;
        Vector2 terrainSize = new Vector2(terrainData.size.x, terrainData.size.y);
        Vector2 normPos = terrainPos / terrainSize;
        float slope = terrainData.GetSteepness(normPos.x, normPos.y);

        // If the slope is up and >45 degrees, that's too much
        if (slope > 45 && terrainModifiedHeight > targetPosition.y)
            Destroy(gameObject);
        else // Otherwise, try some fancy shenanigans to allow to flow over bridges
        {
            // Move up by one to allow for some leeway in obstacle height
            targetPosition.y += 1;

            int terrainLayerMask = 1 << 7;

            RaycastHit hit;
            if (Physics.Raycast(targetPosition, Vector3.down, out hit, 20, terrainLayerMask))
            {
                Debug.DrawRay(targetPosition, Vector3.down * hit.distance, Color.cyan);
                if (Vector3.Angle(hit.normal, Vector3.up) > 45)
                    Destroy(gameObject);
                else
                    targetPosition = hit.point;
            }
            transform.position = targetPosition;
        }
    }
}
