using System;
using UnityEngine;

namespace Builder
{
    public class FreezingBall : InteractiveObject
    {
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player") && isFreezing)
            {
                StartCoroutine(other.gameObject.GetComponent<DroneBuilderController>().IsFreezing());
            }
        }
    }
}