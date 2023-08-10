using System;
using Drone.Builder;
using UnityEngine;

namespace Drone
{
    public class ParticleEffect : MonoBehaviour
    {
        [SerializeField] private ParticleSystem effect;

        public ParticleSystem Effect
        {
            get => effect;
            set => effect = value;
        }

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