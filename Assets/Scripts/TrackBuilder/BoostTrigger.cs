using System.Collections;
using UnityEngine;

namespace Drone.Builder
{
    public class BoostTrigger : InteractiveObject
    {
        public float boostSpeed;
        [SerializeField] private AudioSource boostSound;
        [SerializeField] private Renderer objectRenderer;
        [SerializeField] private float glowIntensity = 1f;
        [SerializeField] private ColorOption selectedColorOption;
        
        private void Start()
        {
            boostSpeed = 2f;
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
            SetColor(GetColorFromOption((ColorOption)colorIndex), objectRenderer, glowIntensity);
        }

        public override void SetColorIndex(int value)
        {
            colorIndex = value;
            SetColor(GetColorFromOption((ColorOption)value), objectRenderer, glowIntensity);
        }
    }
}