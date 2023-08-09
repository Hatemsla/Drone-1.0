using UnityEngine;

namespace Builder
{
    public class BuilderCheckpointTrigger : MonoBehaviour
    {
        public int checkpointId;

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponentInParent<DroneBuilderCheckNode>();
            if (!player || player.currentNode != checkpointId) return;

            player.SetRespawn(transform);
            player.CheckWaypoint();
        }
    }
}