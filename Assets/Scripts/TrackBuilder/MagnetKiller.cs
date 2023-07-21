using System;
using Drone;
using UnityEngine;

namespace Builder
{
    public class MagnetKiller : InteractiveObject
    {
        public float rotationSpeed;
        public Transform leftRotor;
        public Transform rightRotor;
        
        private readonly float _baseDamage = 10.0f;
        private readonly float _damageInterval = 1.0f;
        private float _timer;

        private void FixedUpdate()
        {
            if (isActive)
            {
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

                if (_timer >= _damageInterval)
                {
                    var damage = _baseDamage + (_timer / _damageInterval) * _baseDamage;

                    player.ApplyDamage(damage);
                    _timer = 0.0f;
                }
            }
        }

        public override void SetActive(bool active)
        {
            isActive = active;
        }
    }
}