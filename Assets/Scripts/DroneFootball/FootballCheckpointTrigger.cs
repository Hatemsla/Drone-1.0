using System;
using System.Collections;
using UnityEngine;

namespace DroneFootball
{
    public class FootballCheckpointTrigger : MonoBehaviour
    {
        public float currentDifficultScale;
        private FootballGate _footballGate;

        private void Start()
        {
            _footballGate = GetComponentInParent<FootballGate>();
            transform.localScale = new Vector3(currentDifficultScale, currentDifficultScale, currentDifficultScale);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                transform.localScale = new Vector3(currentDifficultScale, currentDifficultScale, currentDifficultScale);
                var drone = other.GetComponentInParent<DroneFootballCheckNode>();
                drone.CheckWaypoint();
                _footballGate.SetNewGatePosition();
            }
        }
    }
}