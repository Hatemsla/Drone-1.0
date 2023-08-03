using System;
using System.Collections.Generic;
using UnityEngine;

namespace Builder
{
    public class RigidbodyMagnet : InteractiveObject
    {
        private List<Rigidbody> _caughtRigidbodies = new();

        private void Start()
        {
            magnetForce = 1;
        }

        private void FixedUpdate()
        {
            if (isActive)
            {
                if (_caughtRigidbodies != null)
                    foreach (var caughtRb in _caughtRigidbodies)
                        caughtRb.velocity +=
                            (transform.position -
                             (caughtRb.transform.position + caughtRb.centerOfMass)) *
                            (magnetForce * Time.deltaTime);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!isActive)
                return;
            
            if (other.GetComponent<Rigidbody>())
            {
                var r = other.GetComponent<Rigidbody>();

                if (!_caughtRigidbodies.Contains(r)) _caughtRigidbodies.Add(r);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if(!isActive)
                return;
            
            if (other.GetComponent<Rigidbody>())
            {
                var r = other.GetComponent<Rigidbody>();

                if (_caughtRigidbodies.Contains(r))
                    _caughtRigidbodies.Remove(r);
            }
        }

        public override void SetActive(bool active)
        {
            isActive = active;
        }

        public override void SetColorIndex(int active)
        {
        }
    }
}