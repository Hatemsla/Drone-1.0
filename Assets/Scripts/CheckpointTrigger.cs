using System.Collections;
using DroneRace;
using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    public int checkpointID;

    private bool _isSpawned;
    private int _countToDestroy;
    
    private void OnTriggerEnter(Collider other)
    {
        _countToDestroy++;
        if (_countToDestroy >= 2)
        {
            StartCoroutine(DestroyGate());
        }
        
        if (other.gameObject.CompareTag("Player"))
        {
            var drone = other.GetComponentInParent<DroneRaceCheckNode>();
            if (drone.currentNode == checkpointID)
            {
                drone.CheckWaypoint();
                if(!_isSpawned)
                    drone.CreateNewCheckpoint();
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

    private IEnumerator DestroyGate()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}