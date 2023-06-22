using System;
using Drone;
using UnityEngine;

namespace Builder
{
    public class Coin : MonoBehaviour
    {
        public int coin = 1;

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponentInParent<DroneRpgController>();
            if (player)
            {
                player.DroneData.Coins += coin;
                if(BuilderManager.Instance.isGameMode)
                    Destroy(transform.root);
                else
                    transform.root.gameObject.SetActive(false);
            }
        }
    }
}