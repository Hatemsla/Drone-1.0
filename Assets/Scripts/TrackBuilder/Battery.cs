using System;
using Drone;
using UnityEngine;

namespace Builder
{
    public class Battery : InteractiveObject
    {
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
                EffectsManager.Intsance.Get(transform.position);
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
    }
}