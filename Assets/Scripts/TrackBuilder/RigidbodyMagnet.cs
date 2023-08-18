using System;
using System.Collections.Generic;
using UnityEngine;

namespace Drone.Builder
{
    public class RigidbodyMagnet : InteractiveObject
    {
        [SerializeField] private AudioSource workSound;

        private List<Rigidbody> _caughtRigidbodies = new();
        public GameObject colorObject;
        private Renderer objectRenderer;
        public float glowIntensity = 1f;
        public ColorOption selectedColorOption;


        private void SetColor(Color newColor)
        {
            if (isActive)
            {
                objectRenderer.materials[0].SetColor("_Color", newColor);
                objectRenderer.materials[0].EnableKeyword("_EMISSION");
                objectRenderer.materials[0].SetColor("_EmissionColor", newColor * glowIntensity);
            }
            else
            {
                objectRenderer.materials[0].SetColor("_Color", newColor);
                objectRenderer.materials[0].DisableKeyword("_EMISSION");

            }
        }

        private void Start()
        {
            magnetForce = 1;
            objectRenderer = colorObject.GetComponent<Renderer>();
            SetColor(GetColorFromOption(selectedColorOption));

            BuilderManager.Instance.TestLevelEvent += TurnSound;
        }
        private void Update()
        {
            if (CheckColorActivChange(selectedColorOption))
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
            SetColor(GetColorFromOption((ColorOption)color_index));
        }

        public override void SetColorIndex(int value)
        {
            color_index = value;
            SetColor(GetColorFromOption((ColorOption)value));
        }
    }
}