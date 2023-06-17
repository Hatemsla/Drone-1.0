using System;
using UnityEngine;

namespace Builder
{
    public class MagnetKiller : MonoBehaviour
    {
        public float rotationSpeed;
        public Transform leftRotor;
        public Transform rightRotor;

        private void FixedUpdate()
        {
            var leftRotation = rotationSpeed * Time.deltaTime;
            var rightRotation = rotationSpeed * Time.deltaTime;
            
            leftRotor.Rotate(leftRotation, 0f, 0f);
            rightRotor.Rotate(rightRotation, 0f, 0f);
        }

        private void OnTriggerStay(Collider other)
        {
            // var player = other.GetComponent<>;
        }
    }
}