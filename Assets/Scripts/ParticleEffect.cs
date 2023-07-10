using System;
using UnityEngine;

namespace Drone
{
    public class ParticleEffect : MonoBehaviour
    {
        public ParticleSystem effect;

        private void Start()
        {
            effect = GetComponent<ParticleSystem>();
            effect.Play();
        }

        private void OnParticleSystemStopped()
        {
            Destroy(gameObject);
        }
    }
}