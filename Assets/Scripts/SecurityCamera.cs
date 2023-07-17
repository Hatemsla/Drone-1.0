using System;
using Cinemachine;
using UnityEngine;

namespace Drone
{
    public class SecurityCamera : MonoBehaviour
    {
        private CinemachineVirtualCamera _camera;

        private void Start()
        {
            _camera = GetComponent<CinemachineVirtualCamera>();
        }

        public void SetPriority(int priority)
        {
            _camera.Priority = priority;
        }
    }
}