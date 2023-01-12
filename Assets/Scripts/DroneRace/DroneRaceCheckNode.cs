using System.Collections.Generic;
using UnityEngine;

namespace DroneRace
{
    public class DroneRaceCheckNode : MonoBehaviour
    {
        public int currentNode;
        public float wayDistance;
        public List<Transform> nodes;
        public GameObject checkPointPrefab;

        private void Update()
        {
            wayDistance = Vector3.Distance(transform.position, nodes[currentNode].position);
        }

        public void CheckWaypoint()
        {
            currentNode++;
        }

        public void CreateNewCheckpoint()
        {
            var prevCheckpointPosition = nodes[^1].position;
            var newCheckpoint = Instantiate(checkPointPrefab,
                new Vector3(
                    Random.Range(prevCheckpointPosition.x - 5f, prevCheckpointPosition.x + 5f),
                    Random.Range(3f, 10f), 
                    Random.Range(prevCheckpointPosition.z + 5f, prevCheckpointPosition.z + 20f)), 
                Quaternion.identity);
            newCheckpoint.GetComponent<CheckpointTrigger>().checkpointID =
                nodes[^1].GetComponent<CheckpointTrigger>().checkpointID + 1;
            nodes.Add(newCheckpoint.transform);
        }
    }
}