using Drone;
using UnityEngine;

namespace Drone.Builder
{
    public class Crystal : MonoBehaviour
    {
        [SerializeField] private int crystal = 1;

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponentInParent<DroneRpgController>();
            if (player)
            {
                EffectsManager.Intsance.GetGetEffect(transform.position);
                player.Crystals += crystal;
                if(BuilderManager.Instance.isGameMode)
                    Destroy(transform.root);
                else
                    transform.root.gameObject.SetActive(false);
            }
        }
    }
}