using System.Collections;
using System.Collections.Generic;
using FreeMode;
using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    public int checkpointID;
    public MeshRenderer mesh;

    private void Start()
    {
        checkpointID = int.Parse(gameObject.name);
        mesh = GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            var drone = other.GetComponentInParent<CheckNode>();
            var freeModeController = FindObjectOfType<FreeModeController>();
            if (drone.currentNode == checkpointID)
            {
                drone.CheckWaypoint();
                freeModeController.CheckScore();
                mesh.material.color = new Color(0.5f, 0.5f, 0.5f, 0.6f);
            }
        }
    }
}
