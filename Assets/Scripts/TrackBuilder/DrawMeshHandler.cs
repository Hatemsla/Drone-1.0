using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Builder
{
    public class DrawMeshHandler : InteractiveObject
    {
        [SerializeField] private Transform meshParent;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private MeshRenderer[] meshRenderers;

        private Camera _mainCamera;
        
        private void Start()
        {
            _mainCamera = Camera.main;
        }

        public void StartDraw()
        {
            DrawMesh.Instance.meshParent = meshParent.transform;
            _mainCamera.orthographic = true;
            virtualCamera.Priority = 11;
        }

        public void StopDraw()
        {
            DrawMesh.Instance.meshParent = null;
            _mainCamera.orthographic = false;
            virtualCamera.Priority = 0;
        }
    }
}