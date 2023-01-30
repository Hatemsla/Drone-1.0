using System;
using System.Collections;
using UnityEngine;

namespace DroneFootball
{
    public class FootballCheckpointTrigger : MonoBehaviour
    {
        public float currentGateScale;
        public AudioSource music;

        private AudioClip _clip;
        private FootballGate _footballGate;

        private void Start()
        {
            _footballGate = GetComponentInParent<FootballGate>();
            transform.localScale = new Vector3(currentGateScale, currentGateScale, currentGateScale);
            _clip = music.clip;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                music.PlayOneShot(_clip);

                transform.localScale = new Vector3(currentGateScale, currentGateScale, currentGateScale);
                var drone = other.GetComponentInParent<DroneFootballCheckNode>();
                drone.CheckWaypoint();
                _footballGate.SetNewGatePosition();
            }
        }
    }
}