using System;
using System.Collections.Generic;
using UnityEngine;

namespace Drone.Builder
{
    public class RigidbodyMagnet : InteractiveObject
    {
        [SerializeField] private AudioSource workSound;
        
        private List<Rigidbody> _caughtRigidbodies = new();

        private void Start()
        {
            magnetForce = 1;
            BuilderManager.Instance.TestLevelEvent += TurnSound;
        }

        private void OnDestroy()
        {
            BuilderManager.Instance.TestLevelEvent -= TurnSound;
        }

        private void TurnSound()
        {
            if(isActive && BuilderManager.Instance.isMove)
                workSound.Play();
            else
                workSound.Stop();
        }

        private void FixedUpdate()
        {
            if (isActive && BuilderManager.Instance.isMove)
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