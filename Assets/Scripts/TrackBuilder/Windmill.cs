using System;
using Drone;
using UnityEngine;

namespace Drone.Builder
{
    public class Windmill : InteractiveObject
    {
        [SerializeField] private float minWindmillPitch;
        [SerializeField] private float maxWindmillPitch;
        [SerializeField] private Vector3 rotateDirection;
        [SerializeField] private Rigidbody trap;
        public GameObject colorObject;
        private Renderer objectRenderer;
        public float glowIntensity = 1f;
        public ColorOption selectedColorOption;

        [SerializeField] private AudioSource workSound;

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
            
            objectRenderer = colorObject.GetComponent<Renderer>();
            SetColor(GetColorFromOption(selectedColorOption));
            windMillRotateSpeed = 300f;
            BuilderManager.Instance.TestLevelEvent += TurnSound;
        }

        private void Update()
        {
            if (CheckColorActivChange(selectedColorOption))
            {
                isActive = !isActive;
                Debug.Log("CheckColorActivChange");
                Debug.Log(isActive);
                Debug.Log(isActivB);
                Debug.Log(previousIsActiv);

                SetActive(isActive);
            } 
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
            objectRenderer = colorObject.GetComponent<Renderer>();
            SetColor(GetColorFromOption(selectedColorOption));
        }

        private void FixedUpdate()
        {
            if (isActive)
            {
                trap.MoveRotation(trap.rotation * Quaternion.Euler(rotateDirection * (windMillRotateSpeed * Time.deltaTime)));
                workSound.pitch = windMillRotateSpeed / 500 * (maxWindmillPitch - minWindmillPitch) + minWindmillPitch; // Конвертация [0, 500] -> [1, 3]
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
            SetColor(GetColorFromOption((ColorOption)color_index));
        }

        public override void SetColorIndex(int value)
        {
            color_index = value;
            SetColor(GetColorFromOption((ColorOption)value));
        }
    }
}