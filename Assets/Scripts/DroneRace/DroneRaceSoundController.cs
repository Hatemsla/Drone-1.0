using System;
using UnityEngine;

namespace DroneRace
{
    public class DroneRaceSoundController : MonoBehaviour
    {
        public AudioSource droneFly;

        private DroneRaceController _droneRaceController;

        private void Start()
        {
            _droneRaceController = GetComponent<DroneRaceController>();
            _droneRaceController.raceController.droneRaceSoundController = this;
        }
    }
}