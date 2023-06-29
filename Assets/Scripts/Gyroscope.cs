using System;
using UnityEngine;

namespace Drone
{
    public class Gyroscope : MonoBehaviour
    {
        [SerializeField] private RectTransform point;
        [SerializeField] private RectTransform lockator;
        [SerializeField] DroneController droneController;

        private void LateUpdate()
        {
            lockator.rotation = Quaternion.Euler(0f, 0f, droneController.transform.eulerAngles.z);

            var posX = -Mathf.Clamp(Mathf.DeltaAngle(0f, droneController.transform.eulerAngles.z), -20, 20);
            var posY = Mathf.Clamp(Mathf.DeltaAngle(0f, droneController.transform.eulerAngles.x), -20, 20);

            var newPosition = new Vector3(posX, posY, 0f);
            point.localPosition = newPosition;
        }
    }
}