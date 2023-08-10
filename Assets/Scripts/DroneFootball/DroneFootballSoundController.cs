using UnityEngine;

namespace Drone.DroneFootball
{
    public class DroneFootballSoundController : MonoBehaviour
    {
        public AudioSource droneFly;

        private DroneFootballController _droneFootballController;

        private void Start()
        {
            _droneFootballController = GetComponent<DroneFootballController>();
            _droneFootballController.footballController.droneFootballSoundController = this;
        }
    }
}