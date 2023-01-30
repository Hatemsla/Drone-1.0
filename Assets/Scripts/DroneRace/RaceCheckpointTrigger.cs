using System;
using System.Collections;
using UnityEngine;

namespace DroneRace
{
    public class RaceCheckpointTrigger : MonoBehaviour
    {
        public int checkpointID;
        public AudioSource music;

        private AudioClip _clip;
        private bool _isSpawned;
        private bool _isMusic;
        private bool _isPlayerFlew;
        private bool _isBotFlew;
        private int _countToDestroy;

        private void Start()
        {
            _clip = music.clip;
        }

        private void OnTriggerEnter(Collider other)
        {
        
            if (other.gameObject.CompareTag("Player"))
            {
                if (!_isPlayerFlew)
                {
                    _countToDestroy++;
                    _isPlayerFlew = true;
                }

                if (!_isMusic)
                {
                    music.PlayOneShot(_clip);
                    _isMusic = true;
                }
                
                var drone = other.GetComponentInParent<DroneRaceCheckNode>();
                if (drone.currentNode == checkpointID)
                {
                    drone.CheckWaypoint();
                    if(!_isSpawned)
                        drone.CreateNewCheckpoint();
                }
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
            }
            _isSpawned = true;
            
            if (_countToDestroy >= 2)
            {
                StartCoroutine(DestroyGate());
            }
        }

        private IEnumerator DestroyGate()
        {
            yield return new WaitForSeconds(3f);
            Destroy(gameObject);
        }
    }
}