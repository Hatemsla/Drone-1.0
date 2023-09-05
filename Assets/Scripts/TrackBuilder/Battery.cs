using System;
using Drone;
using UnityEngine;

namespace Drone.Builder
{
    public class Battery : InteractiveObject
    {
        public float batteryEnergy;
        [SerializeField] private AudioClip pickupSound;
        
        private void Start()
        {
            batteryEnergy = 15f;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!isActive)
                return;
            
            var player = other.GetComponentInParent<DroneRpgController>();
            if (player)
            {
                EffectsManager.Intsance.GetGetEffect(transform.position);
                SoundManager.Instance.GetSound(transform.position, pickupSound);
                player.Battery += batteryEnergy;
                if(BuilderManager.Instance.isGameMode)
                    Destroy(transform.root.gameObject);
                else
                    transform.root.gameObject.SetActive(false);
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