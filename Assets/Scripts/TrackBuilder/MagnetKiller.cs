using System;
using System.Collections.Generic;
using Drone;
using UnityEngine;

namespace Drone.Builder
{
    public class MagnetKiller : InteractiveObject
    {
        public float rotationSpeed;
        public float magnetForce;
        public float baseDamage = 10.0f;
        public float damageInterval = 1.0f;
        [SerializeField] private Magnet magnet;
        [SerializeField] private Transform leftRotor;
        [SerializeField] private Transform rightRotor;
        [SerializeField] private AudioSource workSound;
        [SerializeField] private AudioSource rotorSound;
        [SerializeField] private Renderer objectRenderer;
        [SerializeField] private float glowIntensity = 1f;
        [SerializeField] private ColorOption selectedColorOption;
        private float _damageIntervalTimer;

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
            if (!isActive) return;
            
            magnet.Attraction(magnetForce);
                
            var leftRotation = rotationSpeed * Time.deltaTime;
            var rightRotation = rotationSpeed * Time.deltaTime;

            leftRotor.Rotate(leftRotation, 0f, 0f);
            rightRotor.Rotate(rightRotation, 0f, 0f);
        }
        
        private void OnTriggerStay(Collider other)
        {
            if(!isActive || !BuilderManager.Instance.isMove)
                return;
            
            var player = other.GetComponentInParent<DroneRpgController>();
            if (player)
            {
                _damageIntervalTimer += Time.deltaTime;

                if (_damageIntervalTimer >= damageInterval)
                {
                    var damage = baseDamage + (_damageIntervalTimer / damageInterval) * baseDamage;

                    player.ApplyDamage(damage);
                    _damageIntervalTimer = 0.0f;
                }
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