using System.Collections;
using System.Collections.Generic;
using DroneRace;
using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    public int checkpointID;
    public MeshRenderer mesh;

    private bool _isSpawned;
    
    private void Start()
    {
        mesh = GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var drone = other.GetComponentInParent<DroneRaceCheckNode>();
            if (drone.currentNode == checkpointID)
            {
                drone.CheckWaypoint();
                if(!_isSpawned)
                    drone.CreateNewCheckpoint();
                mesh.material.color = new Color(0.5f, 0.5f, 0.5f, 0.6f);
            }
        }
        else if (other.gameObject.CompareTag("Bot"))
        {
            var droneRaceCheckNode = other.GetComponentInParent<DroneRaceCheckNode>();
            var droneAI = other.GetComponentInParent<DroneRaceAI>();
            if (droneRaceCheckNode.currentNode == checkpointID)
            {
                droneRaceCheckNode.CheckWaypoint();
                droneAI.droneRaceCheckNode = droneRaceCheckNode;
                if(!_isSpawned)
                    droneRaceCheckNode.CreateNewCheckpoint();
            }
        }
        _isSpawned = true;
    }
}