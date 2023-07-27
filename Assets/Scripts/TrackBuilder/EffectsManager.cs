using System;
using Drone;
using ObjectsPool;
using UnityEngine;

namespace Builder
{
    public sealed class EffectsManager : MonoBehaviour
    {
        public static EffectsManager Intsance;

        [SerializeField] private ParticleEffect getEffect;
        [SerializeField] private int effectsCount;
        
        private PoolBase<ParticleEffect> _particleGetEffectsPool;

        private void Awake()
        {
            Intsance = this;
        }

        private void Start()
        {
            _particleGetEffectsPool = new PoolBase<ParticleEffect>(PreloadParticleEffect, GetParticleEffectAction,
                ReturnParticleEffectAction, effectsCount);
        }

        private ParticleEffect PreloadParticleEffect()
        {
            var newEffect = Instantiate(getEffect, Vector3.zero, getEffect.transform.rotation, transform);
            return newEffect;
        }
        
        private void GetParticleEffectAction(ParticleEffect obj)
        {
            obj.gameObject.SetActive(true);
            obj.Play();
        }

        private void ReturnParticleEffectAction(ParticleEffect obj)
        {
            obj.gameObject.SetActive(false);
            obj.Stop();
        }

        public void Return(ParticleEffect particleEffect)
        {
            _particleGetEffectsPool.Return(particleEffect);
        }

        public void Get(Vector3 position)
        {
            var effect = _particleGetEffectsPool.Get();
            effect.transform.position = position;
        }
    }
}