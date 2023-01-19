using System;
using System.Collections;
using UnityEngine;

namespace DroneFootball
{
    public class FootballCheckpointTrigger : MonoBehaviour
    {
        private FootballGate _footballGate;

        private void Start()
        {
            _footballGate = GetComponentInParent<FootballGate>();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                var drone = other.GetComponentInParent<DroneFootballCheckNode>();
                drone.CheckWaypoint();
                _footballGate.SetNewGatePosition();
            }
        }
    }
}