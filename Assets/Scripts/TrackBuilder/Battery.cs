using System;
using Drone;
using UnityEngine;

namespace Builder
{
    public class Battery : MonoBehaviour
    {
        public int energy;

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponentInParent<DroneRpgController>();
            if (player)
            {
                player.DroneData.Battery += energy;
                if(BuilderManager.Instance.isGameMode)
                    Destroy(transform.root.gameObject);
                else
                    transform.root.gameObject.SetActive(false);
            }
        }
    }
}