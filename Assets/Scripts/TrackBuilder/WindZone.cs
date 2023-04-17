using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Builder
{
    public class WindZone : MonoBehaviour
    {
        public float windForce = 10;
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<Rigidbody>().AddForce(transform.forward * windForce);
            }
        }
    }
}