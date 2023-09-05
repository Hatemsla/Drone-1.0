using System;
using UnityEngine;

namespace Drone.Builder
{
    public class FreezingBall : InteractiveObject
    {
        [SerializeField] private AudioSource workSound;
        public GameObject colorObject;
        private Renderer objectRenderer;
        public float glowIntensity = 1f;
        public ColorOption selectedColorOption;

        private void SetColor(Color newColor)
        {
            if (isActive)
            {
                objectRenderer.material.SetColor("_Color", newColor);
                objectRenderer.material.EnableKeyword("_EMISSION");
                objectRenderer.material.SetColor("_EmissionColor", newColor * glowIntensity);
            }
            else
            {
                objectRenderer.material.SetColor("_Color", newColor);
                objectRenderer.material.DisableKeyword("_EMISSION");

            }
        }
        
        private void Start()
        {
            BuilderManager.Instance.TestLevelEvent += TurnSound;
            objectRenderer = colorObject.GetComponent<Renderer>();
            SetColor(GetColorFromOption(selectedColorOption));
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
            SetColor(GetColorFromOption((ColorOption)colorIndex));
        }

        public override void SetColorIndex(int value)
        {
            colorIndex = value;
            SetColor(GetColorFromOption((ColorOption)value));
        }
    }
}