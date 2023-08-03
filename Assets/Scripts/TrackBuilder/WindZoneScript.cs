using System;
using UnityEngine;

namespace Builder
{
    public class WindZoneScript : InteractiveObject
    {
        private void OnTriggerStay(Collider other)
        {
            if(!isActive)
                return;
            
            var player = other.GetComponent<DroneController>();
            if (player)
            {
                player.GetComponent<Rigidbody>().AddForce(transform.forward * windForce);
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