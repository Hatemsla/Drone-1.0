using UnityEngine;

namespace Drone.Builder
{
    public class DroneBuilderSoundController : MonoBehaviour
    {
        public AudioSource droneFly;

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
        
        private void PlaySounds()
        {
            droneFly.Play();
        }
        
        private void StopSounds()
        {
            droneFly.Stop();
        }
    }
}