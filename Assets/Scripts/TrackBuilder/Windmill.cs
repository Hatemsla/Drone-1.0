using System;
using Drone;
using UnityEngine;

namespace Drone.Builder
{
    public class Windmill : InteractiveObject
    {
        [SerializeField] private float minDronePitch;
        [SerializeField] private float maxDronePitch;
        [SerializeField] private Vector3 rotateDirection;
        [SerializeField] private Rigidbody trap;
        [SerializeField] private AudioSource workSound;

        private void Start()
        {
            windMillRotateSpeed = 300f;
            BuilderManager.Instance.TestLevelEvent += TurnSound;
        }

        private void OnDestroy()
        {
            BuilderManager.Instance.TestLevelEvent -= TurnSound;
        }

        private void TurnSound()
        {
            if(isActive && BuilderManager.Instance.isMove)
                workSound.Play();
            else
                workSound.Stop();
        }

        private void FixedUpdate()
        {
            if (isActive)
            {
                trap.MoveRotation(trap.rotation * Quaternion.Euler(rotateDirection * (windMillRotateSpeed * Time.deltaTime)));
                workSound.pitch = windMillRotateSpeed / 500 * (maxDronePitch - minDronePitch) + minDronePitch; // Конвертация [0, 500] -> [1, 3]
            }
        }
        
        private void OnCollisionEnter(Collision other)
        {
            if(!isActive)
                return;
            
            var player = other.transform.root.GetComponentInParent<DroneBuilderController>();
            if (player)
            {
                player.droneRpgController.ApplyDamage(windMillRotateSpeed / 10 / 2);
            }
        }

        public override void SetActive(bool active)
        {
            isActive = active;
        }

        public override void SetColorIndex(int value)
        {

        }
    }
}