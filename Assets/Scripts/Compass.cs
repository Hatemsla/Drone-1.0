using System;
using Builder;
using UnityEngine;
using UnityEngine.UI;

namespace Drone
{
    public class Compass : MonoBehaviour
    {
        public RawImage compassImage;
        public Transform player;

        private void Start()
        {
            player = FindObjectOfType<DroneController>().transform;
        }

        private void Update()
        {
            compassImage.uvRect = new Rect(player.localEulerAngles.y / 360f, 0f, 1f, 1f);
        }
    }
}