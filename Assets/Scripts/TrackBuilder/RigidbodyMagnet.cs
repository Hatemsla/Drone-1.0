using System;
using System.Collections.Generic;
using UnityEngine;

namespace Drone.Builder
{
    public class RigidbodyMagnet : InteractiveObject
    {
        public float magnetForce; 
        [SerializeField] private AudioSource workSound;
        [SerializeField] private Renderer objectRenderer;
        [SerializeField] private float glowIntensity = 1f;
        [SerializeField] private ColorOption selectedColorOption;
        
        private List<Rigidbody> _caughtRigidbodies = new();

        private void Start()
        {
            SetColor(GetColorFromOption(selectedColorOption), objectRenderer, glowIntensity);

            BuilderManager.Instance.TestLevelEvent += TurnSound;
        }
        private void Update()
        {
            if (CheckColorActiveChange(selectedColorOption))
            {
                isActive = !isActive;
                SetActive(isActive);
            } 
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
            SetColor(GetColorFromOption((ColorOption)colorIndex), objectRenderer, glowIntensity);
        }

        public override void SetColorIndex(int value)
        {
            colorIndex = value;
            SetColor(GetColorFromOption((ColorOption)value), objectRenderer, glowIntensity);
        }
    }
}