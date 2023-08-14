using System;
using Drone;
using ObjectsPool;
using UnityEngine;

namespace Drone.Builder
{
    public sealed class EffectsManager : MonoBehaviour
    {
        public static EffectsManager Intsance;

        [SerializeField] private ParticleEffect getEffect;
        [SerializeField] private ParticleEffect sparksEffect;
        [SerializeField] private ParticleEffect explosionEffect;
        [SerializeField] private int getEffectsCount;
        [SerializeField] private int sparksEffectsCount;
        [SerializeField] private int explosionEffectsCount;
        
        private PoolBase<ParticleEffect> _particleGetEffectsPool;
        private PoolBase<ParticleEffect> _particleSparksEffectsPool;
        private PoolBase<ParticleEffect> _particleExplosionEffectsPool;

        private void Awake()
        {
            Intsance = this;
        }

        private void Start()
        {
            _particleGetEffectsPool = new PoolBase<ParticleEffect>(PreloadParticleGetEffect, GetParticleGetEffectAction,
                ReturnParticleGetEffectAction, getEffectsCount);
            _particleSparksEffectsPool = new PoolBase<ParticleEffect>(PreloadParticleSparksEffect, GetParticleSparksEffectAction,
                ReturnParticleSparksEffectAction, sparksEffectsCount);
            _particleExplosionEffectsPool = new PoolBase<ParticleEffect>(PreloadParticleExplosionEffect, GetParticleExplosionEffectAction,
                ReturnParticleExplosionEffectAction, explosionEffectsCount);
        }

        private ParticleEffect PreloadParticleGetEffect()
        {
            var newEffect = Instantiate(getEffect, Vector3.zero, getEffect.transform.rotation, transform);
            return newEffect;
        }
        
        private ParticleEffect PreloadParticleSparksEffect()
        {
            var newEffect = Instantiate(sparksEffect, Vector3.zero, sparksEffect.transform.rotation, transform);
            return newEffect;
        }
        
        private ParticleEffect PreloadParticleExplosionEffect()
        {
            var newEffect = Instantiate(explosionEffect, Vector3.zero, explosionEffect.transform.rotation, transform);
            return newEffect;
        }
        
        private void GetParticleGetEffectAction(ParticleEffect obj)
        {
            obj.gameObject.SetActive(true);
            obj.Play();
        }
        
        private void GetParticleSparksEffectAction(ParticleEffect obj)
        {
            obj.gameObject.SetActive(true);
            obj.Play();
        }
        
        private void GetParticleExplosionEffectAction(ParticleEffect obj)
        {
            obj.gameObject.SetActive(true);
            obj.Play();
        }

        private void ReturnParticleGetEffectAction(ParticleEffect obj)
        {
            obj.gameObject.SetActive(false);
            obj.Stop();
        }
        
        private void ReturnParticleSparksEffectAction(ParticleEffect obj)
        {
            obj.gameObject.SetActive(false);
            obj.Stop();
        }
        
        private void ReturnParticleExplosionEffectAction(ParticleEffect obj)
        {
            obj.gameObject.SetActive(false);
            obj.Stop();
        }
        
        public void Return(ParticleEffect particleEffect)
        {
            if(_particleGetEffectsPool.HasItem(particleEffect))
                _particleGetEffectsPool.Return(particleEffect);
            if(_particleSparksEffectsPool.HasItem(particleEffect))
                _particleSparksEffectsPool.Return(particleEffect);
            if(_particleExplosionEffectsPool.HasItem(particleEffect))
                _particleExplosionEffectsPool.Return(particleEffect);
        }
        
        public void GetGetEffect(Vector3 position)
        {
            var newEffect = _particleGetEffectsPool.Get();
            newEffect.transform.position = position;
        }
        
        public void GetExplosionEffect(Vector3 position, Vector3 scale)
        {
            var newEffect = _particleExplosionEffectsPool.Get();
            newEffect.transform.position = position;
            newEffect.transform.localScale = scale;
        }
        
        public void GetSparksEffect(Vector3 position, float currentPercentSpeed)
        {
            var newEffect = _particleSparksEffectsPool.Get();
            var burst = newEffect.Effect.emission.GetBurst(0);
            burst.count = new ParticleSystem.MinMaxCurve(currentPercentSpeed);
            newEffect.Effect.emission.SetBurst(0, burst);
            newEffect.transform.position = position;
        }
    }
}