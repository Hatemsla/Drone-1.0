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
            if (isActive)
            {
                trap.MoveRotation(trap.rotation *
                                  Quaternion.Euler(rotateDirection * (windMillRotateSpeed * Time.deltaTime)));
            }
        }
        
        private void OnCollisionEnter(Collision other)
        {
            if(!isActive)
                return;
            
            var player = other.transform.root.GetComponentInParent<DroneBuilderController>();
            if (player)
            {
                player.droneRpgController.ApplyDamage(windMillRotateSpeed / 10 / 2);
            }
        }

        public override void SetActive(bool active)
        {
            isActive = active;
        }
    }
}