using System;
using System.Collections.Generic;
using Drone;
using UnityEngine;

namespace Drone.Builder
{
    public class MagnetKiller : InteractiveObject
    {
        public float rotationSpeed;
        [SerializeField] private Magnet magnet;
        [SerializeField] private Transform leftRotor;
        [SerializeField] private Transform rightRotor;
        [SerializeField] private AudioSource workSound;
        [SerializeField] private AudioSource rotorSound;
        [SerializeField] private float baseDamage = 10.0f;
        [SerializeField] private float damageInterval = 1.0f;
        private float _timer;
        public GameObject colorObject;
        private Renderer objectRenderer;
        public float glowIntensity = 1f;
        public ColorOption selectedColorOption;


        private void SetColor(Color newColor)
        {
            if (isActive)
            {
                objectRenderer.material.SetColor("_Color", newColor);
                objectRenderer.material.EnableKeyword("_EMISSION");
                objectRenderer.material.SetColor("_EmissionColor", newColor * glowIntensity);
            }
            else
            {
                objectRenderer.material.SetColor("_Color", newColor);
                objectRenderer.material.DisableKeyword("_EMISSION");

            }
        }

        private void Start()
        {
            rotationSpeed = 100;
            magnetForce = 1;
            objectRenderer = colorObject.GetComponent<Renderer>();
            SetColor(GetColorFromOption(selectedColorOption));
            BuilderManager.Instance.TestLevelEvent += TurnSound;
        }

        private void OnDestroy()
        {
            BuilderManager.Instance.TestLevelEvent -= TurnSound;
        }

        private void TurnSound()
        {
            if (isActive && BuilderManager.Instance.isMove)
            {
                workSound.Play();
                rotorSound.Play();
            }
            else
            {
                workSound.Stop();
                rotorSound.Stop();
            }
        }

        private void FixedUpdate()
        {
            if (isActive)
            {
                magnet.Attraction(magnetForce);
                
                var leftRotation = rotationSpeed * Time.deltaTime;
                var rightRotation = rotationSpeed * Time.deltaTime;

                leftRotor.Rotate(leftRotation, 0f, 0f);
                rightRotor.Rotate(rightRotation, 0f, 0f);
            }
        }
        
        private void OnTriggerStay(Collider other)
        {
            if(!isActive)
                return;
            
            var player = other.GetComponentInParent<DroneRpgController>();
            if (player)
            {
                _timer += Time.deltaTime;

                if (_timer >= damageInterval)
                {
                    var damage = baseDamage + (_timer / damageInterval) * baseDamage;

                    player.ApplyDamage(damage);
                    _timer = 0.0f;
                }
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