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
            var prevCheckpointRotation = nodes[^1].localRotation;
            GameObject newCheckpoint;
            if ((nodes[^1].GetComponent<CheckpointTrigger>().checkpointID + 1) % 5 == 0)
            {
                newCheckpoint = Instantiate(checkPointPrefab, Vector3.one, Quaternion.identity);
                newCheckpoint.transform.SetParent(nodes[^1]);
                newCheckpoint.transform.Rotate(0f,
                    Random.Range(prevCheckpointRotation.y - 60f, prevCheckpointRotation.y + 60f), 0f);
                newCheckpoint.transform.position = new Vector3(
                    Random.Range(prevCheckpointPosition.x - 5f, prevCheckpointPosition.x + 5f),
                    Random.Range(3f, 10f),
                    Random.Range(prevCheckpointPosition.z + 5f, prevCheckpointPosition.z + 20f));
            }
            else
            {
                newCheckpoint = Instantiate(checkPointPrefab, Vector3.one, Quaternion.identity);
                newCheckpoint.transform.SetParent(nodes[^1]);
                newCheckpoint.transform.Rotate(0f, prevCheckpointRotation.eulerAngles.y, 0f, Space.Self);
                newCheckpoint.transform.position = new Vector3(
                    Random.Range(prevCheckpointPosition.x - 5f, prevCheckpointPosition.x + 5f),
                    Random.Range(3f, 10f),
                    Random.Range(prevCheckpointPosition.z + 5f, prevCheckpointPosition.z + 20f));
            }

            newCheckpoint.GetComponent<CheckpointTrigger>().checkpointID =
                nodes[^1].GetComponent<CheckpointTrigger>().checkpointID + 1;
            nodes.Add(newCheckpoint.transform);
        }
    }
}