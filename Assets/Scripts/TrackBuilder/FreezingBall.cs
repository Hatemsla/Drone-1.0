using System;
using UnityEngine;

namespace Drone.Builder
{
    public class FreezingBall : InteractiveObject
    {
        [SerializeField] private AudioSource workSound;
        [SerializeField] private Renderer objectRenderer;
        [SerializeField] private float glowIntensity = 1f;
        [SerializeField] private ColorOption selectedColorOption;
        
        private void Start()
        {
            BuilderManager.Instance.TestLevelEvent += TurnSound;
            SetColor(GetColorFromOption(selectedColorOption), objectRenderer, glowIntensity);
        }
        
        private void Update()
        {
            if (CheckColorActiveChange(selectedColorOption))
            {
                isActive = !isActive;
                SetActive(isActive);
            } 
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
            SetColor(GetColorFromOption((ColorOption)colorIndex), objectRenderer, glowIntensity);
        }

        public override void SetColorIndex(int value)
        {
            colorIndex = value;
            SetColor(GetColorFromOption((ColorOption)value), objectRenderer, glowIntensity);
        }
    }
}