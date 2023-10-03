using System;
using Drone;
using ObjectsPool;
using UnityEngine;

namespace Drone.Builder
{
    public sealed class EffectsManager : MonoBehaviour
    {
        public static EffectsManager Instance;

        [SerializeField] private ParticleEffect getEffect;
        [SerializeField] private ParticleEffect collisionEffect;
        [SerializeField] private ParticleEffect explosionEffect;
        [SerializeField] private ParticleEffect brokenLampEffect;
        [SerializeField] private int getEffectsCount;
        [SerializeField] private int collisionEffectsCount;
        [SerializeField] private int explosionEffectsCount;
        [SerializeField] private int brokenLampEffectsCount;
        
        private PoolBase<ParticleEffect> _particleGetEffectsPool;
        private PoolBase<ParticleEffect> _particleCollisionEffectsPool;
        private PoolBase<ParticleEffect> _particleExplosionEffectsPool;
        private PoolBase<ParticleEffect> _particleBrokenLampEffectsPool;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _particleGetEffectsPool = new PoolBase<ParticleEffect>(PreloadParticleGetEffect, GetParticleGetEffectAction,
                ReturnParticleGetEffectAction, getEffectsCount);
            _particleCollisionEffectsPool = new PoolBase<ParticleEffect>(PreloadParticleCollisionEffect, GetParticleCollisionEffectAction,
                ReturnParticleCollisionEffectAction, collisionEffectsCount);
            _particleExplosionEffectsPool = new PoolBase<ParticleEffect>(PreloadParticleExplosionEffect, GetParticleExplosionEffectAction,
                ReturnParticleExplosionEffectAction, explosionEffectsCount);
            _particleBrokenLampEffectsPool = new PoolBase<ParticleEffect>(PreloadParticleBrokenLampEffect, GetParticleBrokenLampEffectAction,
                ReturnParticleBrokenLampEffectAction, brokenLampEffectsCount);
        }

        private ParticleEffect PreloadParticleBrokenLampEffect()
        {
            var newEffect = Instantiate(brokenLampEffect, Vector3.zero, brokenLampEffect.transform.rotation, transform);
            return newEffect;
        }

        private ParticleEffect PreloadParticleGetEffect()
        {
            var newEffect = Instantiate(getEffect, Vector3.zero, getEffect.transform.rotation, transform);
            return newEffect;
        }
        
        private ParticleEffect PreloadParticleCollisionEffect()
        {
            var newEffect = Instantiate(collisionEffect, Vector3.zero, collisionEffect.transform.rotation, transform);
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
        
        private void GetParticleCollisionEffectAction(ParticleEffect obj)
        {
            obj.gameObject.SetActive(true);
            obj.Play();
        }
        
        private void GetParticleExplosionEffectAction(ParticleEffect obj)
        {
            obj.gameObject.SetActive(true);
            obj.Play();
        }
        
        private void GetParticleBrokenLampEffectAction(ParticleEffect obj)
        {
            obj.gameObject.SetActive(true);
            obj.Play();
        }

        private void ReturnParticleGetEffectAction(ParticleEffect obj)
        {
            obj.gameObject.SetActive(false);
            obj.Stop();
        }
        
        private void ReturnParticleCollisionEffectAction(ParticleEffect obj)
        {
            obj.gameObject.SetActive(false);
            obj.Stop();
        }
        
        private void ReturnParticleExplosionEffectAction(ParticleEffect obj)
        {
            obj.gameObject.SetActive(false);
            obj.Stop();
        }
        
        private void ReturnParticleBrokenLampEffectAction(ParticleEffect obj)
        {
            obj.gameObject.SetActive(false);
            obj.Stop();
        }
        
        public void Return(ParticleEffect particleEffect)
        {
            if(_particleGetEffectsPool.HasItem(particleEffect))
                _particleGetEffectsPool.Return(particleEffect);
            if(_particleCollisionEffectsPool.HasItem(particleEffect))
                _particleCollisionEffectsPool.Return(particleEffect);
            if(_particleExplosionEffectsPool.HasItem(particleEffect))
                _particleExplosionEffectsPool.Return(particleEffect);
            if(_particleBrokenLampEffectsPool.HasItem(particleEffect))
                _particleBrokenLampEffectsPool.Return(particleEffect);
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
        
        public void GetCollisionEffect(Vector3 position, float currentPercentSpeed)
        {
            var newEffect = _particleCollisionEffectsPool.Get();
            var burst = newEffect.Effect.emission.GetBurst(0);
            burst.count = new ParticleSystem.MinMaxCurve(currentPercentSpeed);
            newEffect.Effect.emission.SetBurst(0, burst);
            newEffect.transform.position = position;
        }
        
        public void GetBrokenLampEffect(Vector3 position, Vector3 scale)
        {
            var newEffect = _particleBrokenLampEffectsPool.Get();
            newEffect.transform.position = position;
            var localScale = newEffect.transform.localScale;
            newEffect.transform.localScale =
                new Vector3(localScale.x + scale.x, localScale.y = scale.y, localScale.z + scale.z);
        }
    }
}