using System.Collections;
using System.Collections.Generic;
using DroneRace;
using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    public int checkpointID;
    public MeshRenderer mesh;

    private void Start()
    {
        mesh = GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var drone = other.GetComponentInParent<DroneRaceCheckNode>();
            var freeModeController = FindObjectOfType<DroneRaceController>();
            if (drone.passedNode == checkpointID)
            {
                drone.CheckWaypoint();
                freeModeController.CheckScore();
                mesh.material.color = new Color(0.5f, 0.5f, 0.5f, 0.6f);
            }
        }
    }
}
