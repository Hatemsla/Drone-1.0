using System.Collections.Generic;
using UnityEngine;

namespace DroneRace
{
    public class DroneRaceCheckNode : MonoBehaviour
    {
        public int currentNode;
        public int passedNode;
        public float wayDistance;
        public List<Transform> nodes;
        public GameObject checkPointPrefab;

        private void Update()
        {
            wayDistance = Vector3.Distance(transform.position, nodes[currentNode].position);
        }

        public void CheckWaypoint()
        {
            CreateNewCheckpoint();
            if (currentNode == nodes.Count - 1)
                currentNode = 0;
            else
                currentNode++;
            passedNode++;
        }

        private void CreateNewCheckpoint()
        {
            var prevCheckpointPosition = nodes[^1].position;
            var newCheckpoint = Instantiate(checkPointPrefab,
                new Vector3(
                    Random.Range(prevCheckpointPosition.x - 5f, prevCheckpointPosition.x + 5f),
                    Random.Range(0f, 10f), 
                    Random.Range(prevCheckpointPosition.z + 5f, prevCheckpointPosition.z + 20f)), 
                Quaternion.identity);
            newCheckpoint.GetComponent<CheckpointTrigger>().checkpointID =
                nodes[^1].GetComponent<CheckpointTrigger>().checkpointID + 1;
            nodes.Add(newCheckpoint.transform);
        }
    }
}