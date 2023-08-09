using System;
using UnityEngine;

namespace Builder
{
    public class DroneBuilderCheckNode : CheckNode
    {
        private Vector3 _lastSavePosition;
        private Quaternion _lastSaveRotation;

        private void Start()
        {
            _lastSavePosition = transform.position;
            _lastSaveRotation = transform.rotation;
        }

        private void Update()
        {
            CalculateWayDistance();
        }

        private void CalculateWayDistance()
        {
            if(currentNode >= nodes.Count)
                return;
        }

        public void CheckWaypoint()
        {
            currentNode++;
        }

        public void AddNode(Transform t)
        {
            nodes.Add(t);
            SetCheckpointsId();
        }

        public void RemoveNode(Transform t)
        {
            nodes.Remove(t);
            SetCheckpointsId();
        }

        private void SetCheckpointsId()
        {
            for (var i = 0; i < nodes.Count; i++)
            {
                nodes[i].GetComponent<BuilderCheckpointTrigger>().checkpointId = i;
            }
        }

        public void SetRespawn(Transform respawn)
        {
            _lastSavePosition = respawn.position;
            _lastSaveRotation = respawn.rotation;
        }

        public Vector3 GetRespawnPosition() => _lastSavePosition;
        public Quaternion GetRespawnRotation() => _lastSaveRotation;
    }
}