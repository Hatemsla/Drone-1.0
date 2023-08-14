using System;
using Drone;
using UnityEngine;

namespace Drone.Builder
{
    public class Coin : MonoBehaviour
    {
        [SerializeField] private int coin = 1;

        [SerializeField] private AudioClip pickupSound;

        private void OnTriggerEnter(Collider other)
        {
            if(!BuilderManager.Instance.isMove)
                return;
            
            var player = other.GetComponentInParent<DroneRpgController>();
            if (player)
            {
                EffectsManager.Intsance.GetGetEffect(transform.position);
                SoundManager.Instance.GetSound(transform.position, pickupSound);
                player.Coins += coin;
                if(BuilderManager.Instance.isGameMode)
                    Destroy(transform.root);
                else
                    transform.root.gameObject.SetActive(false);
                
            }
        }
    }
}