using System;
using System.Collections;
using System.Collections.Generic;
using Drone;
using UnityEngine;

namespace Drone.DroneFootball
{
    public class FootballCheckpointTrigger : MonoBehaviour
    {
        public float currentGateScale;
        public AudioSource music;
        public GameObject playerDrone;
        public List<AudioClip> clips;
        
        private FootballGate _footballGate;
        private readonly Vector3 _playerDroneStartPosition = new Vector3(0, 0, -35);
        private int _clipIndex;
        
        private void Start()
        {
            _footballGate = GetComponentInParent<FootballGate>();
            transform.localScale = new Vector3(currentGateScale, currentGateScale, currentGateScale);
            music.clip = clips[_clipIndex];
            music.volume = AudioListener.volume;
        }

        private void OnTriggerExit(Collider other)
        {
            var player = other.transform.root.GetComponent<DroneController>();
            if (player)
            {
                music.clip = clips[_clipIndex];
                music.PlayOneShot(clips[_clipIndex]);
                if (_clipIndex == clips.Count - 1)
                    _clipIndex = 0;
                else
                    _clipIndex++;
                
                transform.localScale = new Vector3(currentGateScale, currentGateScale, currentGateScale);
                _footballGate.SetNewGatePosition();
                player.GetComponent<DroneFootballCheckNode>().currentNode++;
                player.transform.position = _playerDroneStartPosition;
                player.transform.rotation = Quaternion.identity;
                player.yaw = 0;
                _footballGate.droneFootballTransform = player.transform;
            }
        }
    }
}