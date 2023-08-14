using System.Collections;
using UnityEngine;

namespace Drone.Builder
{
    public class BoostTrigger : InteractiveObject
    {
        [SerializeField] private AudioSource boostSound;
        
        private void Start()
        {
            boostSpeed = 2f;
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
        }

        public override void SetColorIndex(int active)
        {
        }
    }
}