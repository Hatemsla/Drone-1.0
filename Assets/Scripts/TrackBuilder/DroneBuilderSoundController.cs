using System;
using UnityEngine;

namespace Drone.Builder
{
    public class DroneBuilderSoundController : MonoBehaviour
    {
        [SerializeField] private float minDronePitch;
        [SerializeField] private float maxDronePitch;
        [SerializeField] private DroneController droneController;
        
        public AudioSource droneFlySound;
        public AudioSource timeRewindSound;
        public AudioSource activateShieldSound;
        public AudioSource deactivateShieldSound;
        public AudioSource flashlightSound;

        private void Start()
        {
            BuilderManager.Instance.StartGame += PlaySounds;
            BuilderManager.Instance.StopGame += StopSounds;
        }

        private void OnDisable()
        {
            BuilderManager.Instance.StartGame -= PlaySounds;
            BuilderManager.Instance.StopGame -= StopSounds;
        }

        private void Update()
        {
            droneFlySound.pitch = droneController.currentPercentSpeed / 100 * (maxDronePitch - minDronePitch) +
                             minDronePitch;  // Конвертация [0, 100] -> [1, 3]
        }

        private void PlaySounds()
        {
            droneFlySound.Play();
        }
        
        private void StopSounds()
        {
            droneFlySound.Stop();
        }
    }
}