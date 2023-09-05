using System.Collections;
using UnityEngine;

namespace Drone.Builder
{
    public class BoostTrigger : InteractiveObject
    {
        public float boostSpeed;
        [SerializeField] private AudioSource boostSound;
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
            boostSpeed = 2f;
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

        private void OnTriggerEnter(Collider other)
        {
            if(!isActive || !BuilderManager.Instance.isMove)
                return;
            
            var player = other.GetComponent<DroneController>();
            if (player)
            {
                if (player.boostsCount < 3)
                {
                    boostSound.Play();
                    StartCoroutine(BoostDrone(other.GetComponent<Rigidbody>(), player));
                }
            }
        }

        private IEnumerator BoostDrone(Rigidbody rb, DroneController drone)
        {
            rb.velocity *= boostSpeed;
            drone.boostsCount++;
            yield return new WaitForSeconds(2);
            rb.velocity /= boostSpeed;
            drone.boostsCount--;
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