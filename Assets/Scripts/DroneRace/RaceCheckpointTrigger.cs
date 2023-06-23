using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DroneRace
{
    public class RaceCheckpointTrigger : MonoBehaviour
    {
        public int checkpointID;
        public AudioSource music;
        public List<AudioClip> clips;

        private RaceController _raceController;
        private bool _isSpawned;
        private bool _isMusic;
        private bool _isPlayerFlew;
        private bool _isBotFlew;
        private int _countToDestroy;

        private void Start()
        {
            _raceController = FindObjectOfType<RaceController>();
            music.clip = clips[_raceController.clipIndex];
            music.volume = AudioListener.volume;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player") && other.GetComponentInParent<DroneRaceCheckNode>().currentNode == checkpointID)
            {
                if (!_isPlayerFlew)
                {
                    _countToDestroy++;
                    _isPlayerFlew = true;
                }

                if (!_isMusic)
                {
                    music.clip = clips[_raceController.clipIndex];
                    music.PlayOneShot(clips[_raceController.clipIndex]);
                    if (_raceController.clipIndex == clips.Count - 1)
                        _raceController.clipIndex = 0;
                    else
                        _raceController.clipIndex++;
                    _isMusic = true;
                }
                
                var drone = other.GetComponentInParent<DroneRaceCheckNode>();
                drone.CheckWaypoint();
                if(!_isSpawned)
                    drone.CreateNewCheckpoint();
                
                _isSpawned = true;
            }
            else if (other.gameObject.CompareTag("Bot"))
            {
                if (!_isBotFlew)
                {
                    _countToDestroy++;
                    _isBotFlew = true;
                }
                
                var droneRaceCheckNode = other.GetComponentInParent<DroneRaceCheckNode>();
                var droneAI = other.GetComponentInParent<DroneRaceAI>();
                if (droneRaceCheckNode.currentNode == checkpointID)
                {
                    droneRaceCheckNode.CheckWaypoint();
                    droneAI.droneRaceCheckNode = droneRaceCheckNode;
                    if(!_isSpawned)
                        droneRaceCheckNode.CreateNewCheckpoint();
                }
                
                _isSpawned = true;
            }

            if (_countToDestroy >= 2)
            {
                StartCoroutine(DestroyGate());
            }
        }

        private IEnumerator DestroyGate()
        {
            yield return new WaitForSeconds(3f);
            gameObject.SetActive(false);
        }
    }
}