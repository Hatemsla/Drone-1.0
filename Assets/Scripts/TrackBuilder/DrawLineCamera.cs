using System;
using System.Collections;
using System.Collections.Generic;
using Drone.Builder;
using Cinemachine;
using UnityEngine;

namespace Drone.Builder
{
    public class DrawLineCamera : MonoBehaviour
    {
        public CinemachineVirtualCamera virtualCamera;
        [SerializeField] private float orthoIncrease = 0.15f;
        [SerializeField] private float farClipPlaneIncrease = 0.5f;

        private TrackObject _trackObject;
        private float _previousScale;

        private void Start()
        {
            _trackObject = GetComponentInParent<TrackObject>();
            _trackObject.ScaleChanged += OnScaleChanged;
            _previousScale = _trackObject.Scale.x;
        }

        private void OnDestroy()
        {
            _trackObject.ScaleChanged -= OnScaleChanged;
        }

        private void OnScaleChanged(Vector3 scale)
        {
            if (_previousScale < scale.x)
            {
                virtualCamera.m_Lens.OrthographicSize += orthoIncrease;
                virtualCamera.m_Lens.FarClipPlane += farClipPlaneIncrease;
            }
            else if(_previousScale > scale.x)
            {
                virtualCamera.m_Lens.OrthographicSize -= orthoIncrease;
                virtualCamera.m_Lens.FarClipPlane -= farClipPlaneIncrease;
            }
        }
    }
}
