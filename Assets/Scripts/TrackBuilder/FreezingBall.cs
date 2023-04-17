using System;
using UnityEngine;

namespace Builder
{
    public class FreezingBall : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                StartCoroutine(other.gameObject.GetComponent<DroneBuilderController>().IsFreezing());
            }
        }
    }
}