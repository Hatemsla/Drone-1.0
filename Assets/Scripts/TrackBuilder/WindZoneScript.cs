using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Builder
{
    public class WindZoneScript : InteractiveObject
    {
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<Rigidbody>().AddForce(transform.forward * windForce);
            }
        }

        public override void SetActive(bool active)
        {
            
        }
    }
}