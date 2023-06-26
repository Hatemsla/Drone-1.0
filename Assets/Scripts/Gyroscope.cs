using System;
using UnityEngine;

namespace Drone
{
    public class Gyroscope : MonoBehaviour
    {
        [SerializeField] private RectTransform point;
        [SerializeField] private RectTransform lockator;
        private DroneController _droneController;
        private readonly float _positionScale = 20f;

        private void Start()
        {
            _droneController = FindObjectOfType<DroneController>();
        }

        private void LateUpdate()
        {
            lockator.rotation = Quaternion.Euler(0f, 0f, _droneController.transform.eulerAngles.z);

            var posX = -Mathf.Clamp(Mathf.DeltaAngle(0f, _droneController.transform.eulerAngles.z), -20, 20);
            var posY = Mathf.Clamp(Mathf.DeltaAngle(0f, _droneController.transform.eulerAngles.x), -20, 20);

            var newPosition = new Vector3(posX, posY, 0f);
            point.localPosition = newPosition;
        }
    }
}