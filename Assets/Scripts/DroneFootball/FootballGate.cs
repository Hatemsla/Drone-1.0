using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Drone.DroneFootball
{
    public class FootballGate : MonoBehaviour
    {
        public FootballController footballController;
        public Transform droneFootballTransform;
        public DroneFootballAI droneFootballAI1;
        public DroneFootballAI droneFootballAI2;
        public Transform botPoint1;
        public Transform botPoint2;

        private Vector3 _circleCenter = new Vector3(0, 5, 0);

        private void Awake()
        {
            SetBotsPositions();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("Player"))
            {
                droneFootballAI1.targetTransform = droneFootballTransform;
                droneFootballAI2.targetTransform = droneFootballTransform;
            }
        }

        public void SetNewGatePosition()
        {
            var y = Random.Range(5f, 20f);
            _circleCenter = new Vector3(0f, y, 0f);
            transform.position = new Vector3(Random.Range(-40f, 40f), y, Random.Range(0, 40f));
            transform.LookAt(_circleCenter);
            SetBotsPositions();
        }

        private void SetBotsPositions()
        {
            droneFootballAI1.targetTransform = botPoint1;
            droneFootballAI1.transform.position = botPoint1.transform.position;
            droneFootballAI2.targetTransform = botPoint2;
            droneFootballAI2.transform.position = botPoint2.transform.position;
        }
    }
}
