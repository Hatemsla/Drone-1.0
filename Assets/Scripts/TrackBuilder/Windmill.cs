using System;
using Drone;
using UnityEngine;

namespace Builder
{
    public class Windmill : MonoBehaviour
    {
        public float rotateSpeed;
        public Rigidbody trap;

        private void FixedUpdate()
        {
            trap.MoveRotation(trap.rotation * Quaternion.Euler(0, Time.deltaTime * rotateSpeed, 0));
        }
    }
}