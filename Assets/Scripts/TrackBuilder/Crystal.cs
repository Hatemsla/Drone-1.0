using Drone;
using UnityEngine;

namespace Drone.Builder
{
    public class Crystal : MonoBehaviour
    {
        [SerializeField] private int crystal = 1;
        [SerializeField] private AudioClip pickupSound;

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponentInParent<DroneRpgController>();
            if (player)
            {
                EffectsManager.Instance.GetGetEffect(transform.position);
                player.Crystals += crystal;
                if(BuilderManager.Instance.isGameMode)
                    Destroy(transform.root);
                else
                    transform.root.gameObject.SetActive(false);
            }
        }
    }
}