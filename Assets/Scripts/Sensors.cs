using System;
using System.Collections;
using Drone;
using UnityEngine;

namespace Drone
{
    public class Sensors : MonoBehaviour
    {
        public float currentHeight;
        public float currentForwardDistance;
        [SerializeField] private LayerMask heightMask;
        [SerializeField] private LayerMask forwardDistanceMask;
        [SerializeField] private float waitTime = 0.1f;
        private float _currentTime;

        private void SendData(float height, float distance)
        {
            StartCoroutine(GameManager.Instance.scratchClient.SendData(height, distance));
        }

        private void Update()
        {
            if (Physics.Raycast(transform.position, -transform.up, out var hitHeight, float.MaxValue, heightMask))
            {
                currentHeight = hitHeight.distance;
            }

            if (Physics.Raycast(transform.position, transform.forward, out var hitDistance, float.MaxValue, forwardDistanceMask))
            {
                currentForwardDistance = hitDistance.distance;
            }

            if (_currentTime >= waitTime)
            {
                _currentTime = 0;
                SendData(currentHeight, currentForwardDistance);
            }

            _currentTime += Time.deltaTime;
        }
    }
}