using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Drone.DroneRace
{
    public class DroneRaceAudioController : MonoBehaviour
    {
        public AudioSource droneFly;
        public AudioMixer audioMixer;

        private DroneRaceController _droneRaceController;

        private void Start()
        {
            audioMixer.SetFloat("Music", 0);
            audioMixer.SetFloat("Effects", 0);
            _droneRaceController = GetComponent<DroneRaceController>();
            _droneRaceController.raceController.droneRaceAudioController = this;
        }
    }
}