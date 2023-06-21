using System;
using Drone;
using UnityEngine;

namespace Builder
{
    public class WindmillAttacker : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            var player = other.transform.root.GetComponentInParent<DroneBuilderController>();
            if (player)
            {
                if (player.currentPercentSpeed >= 50f)
                {
                    player.droneRpgController.ApplyDamage((int)player.currentSpeed);
                }
            }
        }
    }
}