using System;
using Drone;
using UnityEngine;

namespace Builder
{
    public class Battery : InteractiveObject
    {
        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponentInParent<DroneRpgController>();
            if (player)
            {
                player.DroneData.Battery += batteryEnergy;
                if(BuilderManager.Instance.isGameMode)
                    Destroy(transform.root.gameObject);
                else
                    transform.root.gameObject.SetActive(false);
            }
        }
    }
}