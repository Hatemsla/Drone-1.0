using System;
using Drone.Builder;
using UnityEngine;
using UnityEngine.UI;

namespace Drone
{
    public class Compass : MonoBehaviour
    {
        [SerializeField] private RawImage compassImage;
        [SerializeField] private Transform player;

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