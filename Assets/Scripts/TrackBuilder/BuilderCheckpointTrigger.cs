using UnityEngine;

namespace Drone.Builder
{
    public class BuilderCheckpointTrigger : MonoBehaviour
    {
        public int checkpointId;
        [SerializeField] private AudioSource saveSound;

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponentInParent<DroneBuilderCheckNode>();
            if (!player || player.currentNode != checkpointId) return;

            player.SetRespawn(transform);
            player.CheckWaypoint();
            saveSound.Play();
        }
    }
}