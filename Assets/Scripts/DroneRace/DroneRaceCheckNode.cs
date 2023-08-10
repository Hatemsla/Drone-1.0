using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Drone.DroneRace
{
    public class DroneRaceCheckNode : CheckNode
    {
        public GameObject checkPointPrefab;

        private RacePath _racePath;

        private void Start()
        {
            _racePath = FindObjectOfType<RacePath>();
        }

        private void Update()
        {
            wayDistance = Vector3.Distance(transform.position, nodes[currentNode].position);
        }

        public void CheckWaypoint()
        {
            nodes = _racePath.nodes;
            currentNode++;
        }

        public void CreateNewCheckpoint()
        {
            var prevCheckpointPosition = nodes[^1].position;
            GameObject newCheckpoint;
            if ((nodes[^1].GetComponent<RaceCheckpointTrigger>().checkpointID + 1) % 5 == 0)
            {
                newCheckpoint = Instantiate(checkPointPrefab, Vector3.zero, Quaternion.identity);
                newCheckpoint.transform.SetParent(nodes[^1]);
                newCheckpoint.transform.localEulerAngles = new Vector3(0f, Random.Range(-50f, 50f), 0f);
                if (prevCheckpointPosition.y < 10)
                {
                    newCheckpoint.transform.localPosition = new Vector3(
                        Random.Range(-2f, 2f),
                        Random.Range(0f, 4f),
                        Random.Range(5f, 10f));
                }
                else
                {
                    newCheckpoint.transform.localPosition = new Vector3(
                        Random.Range(-2f, 2f),
                        Random.Range(-2f, 2f),
                        Random.Range(5f, 10f));
                }

                newCheckpoint.transform.parent = null;
                newCheckpoint.transform.localScale = new Vector3(3, 3, 3);
            }
            else
            {
                newCheckpoint = Instantiate(checkPointPrefab, Vector3.zero, Quaternion.identity);
                newCheckpoint.transform.SetParent(nodes[^1]);
                newCheckpoint.transform.localEulerAngles = Vector3.zero;
                if (prevCheckpointPosition.y < 10)
                {
                    newCheckpoint.transform.localPosition = new Vector3(
                        Random.Range(-2f, 2f),
                        Random.Range(0f, 4f),
                        Random.Range(5f, 10f));
                }
                else
                {
                    newCheckpoint.transform.localPosition = new Vector3(
                        Random.Range(-2f, 2f),
                        Random.Range(-2f, 2f),
                        Random.Range(5f, 10f));
                }
                
                newCheckpoint.transform.parent = null;
                newCheckpoint.transform.localScale = new Vector3(3, 3, 3);
            }

            newCheckpoint.GetComponent<RaceCheckpointTrigger>().checkpointID =
                _racePath.nodes[^1].GetComponent<RaceCheckpointTrigger>().checkpointID + 1;
            _racePath.nodes.Add(newCheckpoint.transform);
        }
    }
}