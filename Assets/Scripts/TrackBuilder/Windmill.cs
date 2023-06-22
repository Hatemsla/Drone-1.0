using System;
using Drone;
using UnityEngine;

namespace Builder
{
    public class Windmill : MonoBehaviour
    {
        public Vector3 rotateDirection;
        public float rotateSpeed;
        public Rigidbody trap;

        private void FixedUpdate()
        {
            trap.MoveRotation(trap.rotation * Quaternion.Euler(rotateDirection * (rotateSpeed * Time.deltaTime)));
        }
        
        private void OnCollisionEnter(Collision other)
        {
            var player = other.transform.root.GetComponentInParent<DroneBuilderController>();
            if (player)
            {
                player.droneRpgController.ApplyDamage(rotateSpeed / 10 / 2);
            }
        }
    }
}