using UnityEngine;

namespace Builder
{
    public class BuilderCheckpointTrigger : MonoBehaviour
    {
        public int checkpointId;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player") ||
                other.GetComponentInParent<DroneBuilderCheckNode>().currentNode != checkpointId) return;
            
            var drone = other.GetComponentInParent<DroneBuilderCheckNode>();
            drone.CheckWaypoint();
        }
    }
}