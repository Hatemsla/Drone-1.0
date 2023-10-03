using System;
using Cinemachine;
using Drone;
using UnityEngine;

namespace Drone.Builder
{
    public class BuilderCameraController : MonoBehaviour
    {
        public bool isSwitch;
        public bool isFirstView;
        [SerializeField] private CinemachineVirtualCamera thirdView;
        [SerializeField] private CinemachineVirtualCamera firstView;

        private int _prevThirdView = 10;
        private int _prevFirstView = 0;

        private void OnEnable()
        {
            InputManager.Instance.SwitchViewEvent += SwitchView;
        }
        
        private void OnDisable()
        {
            InputManager.Instance.SwitchViewEvent -= SwitchView;
            BuilderManager.Instance.StartGame -= ActivateCamera;
            BuilderManager.Instance.StopGame -= DeactivateCamera;
        }

        private void Start()
        {
            thirdView = BuilderManager.Instance.thirdPersonCamera;
            thirdView.LookAt = transform;
            thirdView.Follow = transform;
            
            BuilderManager.Instance.StartGame += ActivateCamera;
            BuilderManager.Instance.StopGame += DeactivateCamera;
        }

        private void ActivateCamera()
        {
            isSwitch = true;
            SetUpCamerasDefaultPriority();
        }

        private void DeactivateCamera()
        {
            isSwitch = false;
            SetUpCamerasZeroPriority();
        }

        private void SwitchView()
        {
            if (isSwitch)
            {
                (_prevThirdView, _prevFirstView) = (_prevFirstView, _prevThirdView);
                thirdView.Priority = _prevThirdView;
                firstView.Priority = _prevFirstView;
                isFirstView = firstView.Priority > thirdView.Priority;
            }
        }

        private void SetUpCamerasDefaultPriority()
        {
            thirdView.Priority = 10;
            firstView.Priority = 0;
        }

        private void SetUpCamerasZeroPriority()
        {
            thirdView.Priority = 0;
            firstView.Priority = 0;
        }
    }
}