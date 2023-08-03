using System;
using System.Collections.Generic;
using Drone;
using UnityEngine;

namespace Builder
{
    public class MagnetKiller : InteractiveObject
    {
        public float rotationSpeed;
        [SerializeField] private Magnet magnet;
        [SerializeField] private Transform leftRotor;
        [SerializeField] private Transform rightRotor;
        [SerializeField] private float baseDamage = 10.0f;
        [SerializeField] private float damageInterval = 1.0f;
        private float _timer;

        private void Start()
        {
            rotationSpeed = 100;
            magnetForce = 1;
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
        }

        public override void SetColorIndex(int value)
        {

        }
    }
}