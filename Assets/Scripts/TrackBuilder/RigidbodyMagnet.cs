using System.Collections.Generic;
using UnityEngine;

namespace Builder
{
    public class RigidbodyMagnet : InteractiveObject
    {
        private List<Rigidbody> _caughtRigidbodies = new();

        private void FixedUpdate()
        {
            if (_caughtRigidbodies != null)
                foreach (var caughtRb in _caughtRigidbodies)
                    caughtRb.velocity +=
                        (transform.position -
                         (caughtRb.transform.position + caughtRb.centerOfMass)) *
                        (magnetForce * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Rigidbody>())
            {
                var r = other.GetComponent<Rigidbody>();

                if (!_caughtRigidbodies.Contains(r)) _caughtRigidbodies.Add(r);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<Rigidbody>())
            {
                var r = other.GetComponent<Rigidbody>();

                if (_caughtRigidbodies.Contains(r))
                    _caughtRigidbodies.Remove(r);
            }
        }
    }
}