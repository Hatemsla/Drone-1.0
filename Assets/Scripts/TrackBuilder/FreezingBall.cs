using System;
using UnityEngine;

namespace Drone.Builder
{
    public class FreezingBall : InteractiveObject
    {
        [SerializeField] private AudioSource workSound;

        private void Start()
        {
            BuilderManager.Instance.TestLevelEvent += TurnSound;
        }

        private void OnDestroy()
        {
            BuilderManager.Instance.TestLevelEvent -= TurnSound;
        }

        private void TurnSound()
        {
            if (isActive && BuilderManager.Instance.isMove)
                workSound.Play();
            else
                workSound.Stop();
        }

        private void OnCollisionEnter(Collision other)
        {
            if(!isActive || !BuilderManager.Instance.isMove)
                return;
            
            var player = other.gameObject.GetComponent<DroneBuilderController>();
            if (player)
            {
                StartCoroutine(player.IsFreezing());
            }
        }

        public override void SetActive(bool active)
        {
            isActive = active;
        }

        public override void SetColorIndex(int active)
        {
        }
    }
}