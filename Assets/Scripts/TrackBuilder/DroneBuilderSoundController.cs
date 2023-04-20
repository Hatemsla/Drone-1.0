using System;
using UnityEngine;

namespace Builder
{
    public class DroneBuilderSoundController : MonoBehaviour
    {
        public AudioSource droneFly;

        private DroneBuilderController _droneBuilderController;
        
        private void Start()
        {
            _droneBuilderController = GetComponent<DroneBuilderController>();
            _droneBuilderController.builderManager.droneBuilderSoundController = this;
        }
    }
}