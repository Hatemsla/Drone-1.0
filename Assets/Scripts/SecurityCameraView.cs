using System;
using Builder;
using TMPro;
using UnityEngine;

namespace Drone
{
    public class SecurityCameraView : MonoBehaviour
    {
        [SerializeField] public TMP_Text cameraNumber;

        private Port _currentPort;

        private void OnEnable()
        {
            _currentPort.ChangeSecurityCameraEvent += UpdateCameraNumber;
        }

        private void OnDisable()
        {
            _currentPort.ChangeSecurityCameraEvent -= UpdateCameraNumber;
        }

        public void GetPort(Port port)
        {
            _currentPort = port;
        }

        private void UpdateCameraNumber(int value)
        {
            cameraNumber.text = $"CAM {value:00}";
        }
    }
}