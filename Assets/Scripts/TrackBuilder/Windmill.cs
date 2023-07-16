using System;
using Drone;
using UnityEngine;

namespace Builder
{
    public class Windmill : InteractiveObject
    {
        [SerializeField] private Vector3 rotateDirection;
        [SerializeField] private Rigidbody trap;

        private void FixedUpdate()
        {
            trap.MoveRotation(trap.rotation * Quaternion.Euler(rotateDirection * (windMillRotateSpeed * Time.deltaTime)));
        }
        
        private void OnCollisionEnter(Collision other)
        {
            var player = other.transform.root.GetComponentInParent<DroneBuilderController>();
            if (player)
            {
                player.droneRpgController.ApplyDamage(windMillRotateSpeed / 10 / 2);
            }
        }
    }
}