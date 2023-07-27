using System;
using Builder;
using UnityEngine;

namespace Drone
{
    public class ParticleEffect : MonoBehaviour
    {
        [SerializeField] private ParticleSystem effect;
        
        private void OnParticleSystemStopped()
        {
            EffectsManager.Intsance.Return(this);
        }

        public void Play()
        {
            effect.Play();
        }

        public void Stop()
        {
            effect.Stop();
        }
    }
}