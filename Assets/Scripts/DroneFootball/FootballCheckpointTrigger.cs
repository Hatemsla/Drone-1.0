using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DroneFootball
{
    public class FootballCheckpointTrigger : MonoBehaviour
    {
        public float currentGateScale;
        public AudioSource music;
        public GameObject playerDrone;
        public List<AudioClip> clips;
        
        private FootballGate _footballGate;
        private readonly Vector3 _playerDroneStartPosition = new Vector3(0, 0, -35);
        private int _playerScore;
        private int _clipIndex;
        private float _yawPower;
        private Color _playerColor;
        
        private void Start()
        {
            _footballGate = GetComponentInParent<FootballGate>();
            transform.localScale = new Vector3(currentGateScale, currentGateScale, currentGateScale);
            music.clip = clips[_clipIndex];
            music.volume = AudioListener.volume;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                music.clip = clips[_clipIndex];
                music.PlayOneShot(clips[_clipIndex]);
                if (_clipIndex == clips.Count - 1)
                    _clipIndex = 0;
                else
                    _clipIndex++;
                
                transform.localScale = new Vector3(currentGateScale, currentGateScale, currentGateScale);
                _playerScore = other.GetComponent<DroneFootballCheckNode>().currentNode+1;
                _yawPower = other.GetComponent<DroneFootballController>().yawPower;
                _playerColor = other.GetComponent<DroneFootballController>().droneMeshRenderer.material.color;
                _footballGate.SetNewGatePosition();
                Destroy(other.gameObject);
                var player = Instantiate(playerDrone, _playerDroneStartPosition, Quaternion.identity);
                var playerCheckNode = player.GetComponent<DroneFootballCheckNode>();
                var playerController = player.GetComponent<DroneFootballController>();
                playerCheckNode.currentNode = _playerScore;
                playerController.footballController.playerCheckNode = playerCheckNode;
                playerController.yawPower = _yawPower;
                playerController.droneMeshRenderer.material.SetColor("_Color", _playerColor);
                playerController.droneMeshRenderer.material.SetColor("_EmissionColor", _playerColor);
                playerController.footballController.CheckScore();
                _footballGate.droneFootballTransform = player.transform;
            }
        }
    }
}