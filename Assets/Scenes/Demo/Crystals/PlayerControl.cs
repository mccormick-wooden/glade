using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Abstract;
using UnityEngine;

namespace crystal
{
    public class PlayerControl : MonoBehaviour
    {
        [SerializeField]
        private float PlayerSpeed = 1;

        [SerializeField]
        private CrystalManager crystalManager;

        private Vector3 targetVelocity;

        Rigidbody rb;
        void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 targetVelocity = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                targetVelocity += transform.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                targetVelocity += -1 * transform.forward;
            }
            if (Input.GetKey(KeyCode.A))
            {
                targetVelocity += -1 * transform.right;
            }
            if (Input.GetKey(KeyCode.D))
            {
                targetVelocity += transform.right;
            }
            if (Input.GetKeyUp(KeyCode.N))
            {
                GameObject crystal = crystalManager.FindNearestCrystal(transform.position);
                Debug.Log($"Found crystal {crystal.name}");
            }

            targetVelocity.Normalize();
            targetVelocity *= PlayerSpeed;
            rb.velocity = targetVelocity;
        }

        private void FixedUpdate()
        {
        }
    }
}
