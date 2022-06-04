using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFootPlacement : MonoBehaviour
{

    Animator anim;
    public LayerMask layerMask; // Select all layers that foot placement applies to.
    public float DistanceToGround; // Distance from where the foot transform is to the lowest possible position of the foot.
    public GameObject player;

    private void Awake()
    {
        anim = player.GetComponent<Animator>();
        
        if (anim == null)
            Debug.LogError($"Couldn't find {GetType().Name}.{nameof(anim)}");
    }

    private void Update()
    {

    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (anim == null)
        {
            return;
        }
        
        // Set the weights of left and right feet to the current value defined by the curve in our animations.
        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, anim.GetFloat("IKLeftFootWeight"));
        anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, anim.GetFloat("IKLeftFootWeight"));
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, anim.GetFloat("IKRightFootWeight"));
        anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, anim.GetFloat("IKRightFootWeight"));
            
        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
        anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);
            

        // Left Foot
        RaycastHit hit;

        // We cast our ray from above the foot in case the current terrain/floor is above the foot position.
        Ray ray = new Ray(anim.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out hit, DistanceToGround + 1f, layerMask))
        {

            // We're only concerned with objects that are tagged as "Walkable"
            if (hit.transform.tag == "Walkable")
            {

                Vector3 footPosition = hit.point; // The target foot position is where the raycast hit a walkable object...
                footPosition.y += DistanceToGround; // ... taking account the distance to the ground we added above.
                anim.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
                anim.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, hit.normal));

            }

        }

        // Right Foot
        ray = new Ray(anim.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out hit, DistanceToGround + 1f, layerMask))
        {

            if (hit.transform.tag == "Walkable")
            {

                Vector3 footPosition = hit.point;
                footPosition.y += DistanceToGround;
                anim.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);
                anim.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, hit.normal));

            }

        }



    }

}