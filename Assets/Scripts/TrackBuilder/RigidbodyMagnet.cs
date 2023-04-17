using System.Collections.Generic;
using UnityEngine;

namespace Builder
{
    public class RigidbodyMagnet : MonoBehaviour
    {
        public float magnetForce = 100;

        private List<Rigidbody> _caughtRigidbodies = new List<Rigidbody>();

        void FixedUpdate()
        {
            for (int i = 0; i < _caughtRigidbodies.Count; i++)
            {
                _caughtRigidbodies[i].velocity += (transform.position - (_caughtRigidbodies[i].transform.position + _caughtRigidbodies[i].centerOfMass)) * magnetForce * Time.deltaTime;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Rigidbody>())
            {
                Rigidbody r = other.GetComponent<Rigidbody>();

                if(!_caughtRigidbodies.Contains(r))
                {
                    //Add Rigidbody
                    _caughtRigidbodies.Add(r);
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<Rigidbody>())
            {
                Rigidbody r = other.GetComponent<Rigidbody>();

                if (_caughtRigidbodies.Contains(r))
                {
                    //Remove Rigidbody
                    _caughtRigidbodies.Remove(r);
                }
            }
        }
    }
}