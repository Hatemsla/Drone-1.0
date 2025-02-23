﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Drone.Builder;
using UnityEngine;

namespace Drone
{
    public class DroneRpgController : MonoBehaviour
    {
        public DroneController droneBuilderController;
        [SerializeField] private DroneData droneData;
        [SerializeField] private DroneBuilderCheckNode droneBuilderCheckNode;
        [SerializeField] private DroneBuilderSoundController droneBuilderSoundController;
        public float powerUsageRate;
        public bool isAlive = true;
        public bool isCharged = true;
        public bool isEnergyUsage = true;
        [SerializeField] private float armorPercentage = 0.9f;
        [SerializeField] private float respawnTime = 3f;

        public event Action<Skills> SkillsCountChangedEvent;

        private bool _isRespawning;
        private bool _isReturnToCheckpoint;

        private float _currentRespawnTime;
        
        private readonly float[] _thresholds = { 0, 20f, 40f, 60f, 80f, 100f };

        public Dictionary<Skills, int> SkillsCount
        {
            get => droneData.skillsCount;
            set => droneData.skillsCount = value;
        }

        public bool IsReset
        {
            get => droneData.IsReset;
            set => droneData.IsReset = value;
        }
        
        public float TimeForEndGame
        {
            get => droneData.TimeForEndGame;
            set => droneData.TimeForEndGame = value;
        }

        public float Battery
        {
            get => droneData.Battery;
            set
            {
                droneData.Battery = value;
                if (value <= 0)
                {
                    isCharged = false;
                    if(!_isRespawning)
                        StartCoroutine(Respawn());
                }
            }
        }

        public float Health
        {
            get => droneData.Health;
            set
            {
                if (value > 100)
                    droneData.Health = 100;
                else
                    droneData.Health = value;

                if (value <= 0)
                {
                    isAlive = false;
                    if(!_isRespawning)
                        StartCoroutine(Respawn());
                }
            }
        }

        public float Armor
        {
            get => droneData.Armor;
            set
            {
                if (value > 100)
                    droneData.Armor = 100;
                else
                    droneData.Armor = value;
            }
        }

        public int Coins
        {
            get => droneData.Coins;
            set => droneData.Coins = value;
        }

        public int Crystals
        {
            get => droneData.Crystals;
            set => droneData.Crystals = value;
        }

        private void OnEnable()
        {
            BuilderManager.Instance.StartGame += ResetDroneData;
            InputManager.Instance.ReturnToCheckpointEvent += ReturnToCheckpoint;
        }

        private void OnDestroy()
        {
            BuilderManager.Instance.StartGame -= ResetDroneData;
            InputManager.Instance.ReturnToCheckpointEvent -= ReturnToCheckpoint;
        }
        
        private void Update()
        {
            if (BuilderManager.Instance.isMove && !RewindManager.Instance.IsBeingRewinded)
                ApplyEnergyUsage(powerUsageRate * Time.deltaTime);

            ReturnToCheckpoint();
        }

        private void ReturnToCheckpoint()
        {
            if (_isReturnToCheckpoint && !_isRespawning)
            {
                if (!BuilderManager.Instance.builderUI.restoreHealthText.enabled)
                    BuilderManager.Instance.builderUI.restoreHealthText.enabled = true;
                _currentRespawnTime += Time.deltaTime;
                var waitTime = respawnTime - _currentRespawnTime;
                BuilderManager.Instance.builderUI.restoreHealthText.text = $"Респавн через: {waitTime:f1}";

                if (_currentRespawnTime >= respawnTime)
                {
                    transform.position = droneBuilderCheckNode.GetRespawnPosition();
                    droneBuilderController.yaw = droneBuilderCheckNode.GetRespawnRotation().eulerAngles.y;
                    BuilderManager.Instance.builderUI.restoreHealthText.enabled = false;
                }
            }
            else
            {
                _currentRespawnTime = 0f;
                BuilderManager.Instance.builderUI.restoreHealthText.enabled = false;
            }
        }

        private void ReturnToCheckpoint(bool value)
        {
            _isReturnToCheckpoint = value;
        }

        private IEnumerator Respawn()
        {
            var elapsedTime = 0f;
            
            _isRespawning = true;
            BuilderManager.Instance.builderUI.restoreHealthText.enabled = true;

            while (elapsedTime < respawnTime)
            {
                elapsedTime += Time.deltaTime;
                var waitTime = respawnTime - elapsedTime;
                BuilderManager.Instance.builderUI.restoreHealthText.text = $"Респавн через: {waitTime:f1}";
                yield return null;
            }
            transform.position = droneBuilderCheckNode.GetRespawnPosition();
            droneBuilderController.yaw = droneBuilderCheckNode.GetRespawnRotation().eulerAngles.y;
            isCharged = true;
            isAlive = true;

            if (Health <= 0)
                Health = 50;
            
            if (Battery <= 0)
                Battery = 50;

            _isRespawning = false;
            BuilderManager.Instance.builderUI.restoreHealthText.enabled = false;
        }

        public void UpdateSkillValue(Skills skill, int newValue)
        {
            SkillsCount[skill] = newValue;
            SkillsCountChangedEvent?.Invoke(skill);
        }

        private void ResetDroneData()
        {
            if (IsReset)
            {
                Battery = 100;
                Health = 100;
                Armor = 100;
                Coins = 0;
                Crystals = 0;
                TimeForEndGame = BuilderManager.Instance.timer.timeForEndGame;
                foreach (var skills in SkillsCount.Keys.ToArray())
                    SkillsCount[skills] = 0;
                foreach (var buyingType in droneData.buyingTypes)
                    buyingType.Value.Clear();    
                IsReset = false;
            }
            else
            {
                BuilderManager.Instance.timer.waitForEndGame = TimeForEndGame;
                IsReset = true;
            }
        }

        public void ApplyEnergyUsage(float energyUsage)
        {
            if(!isEnergyUsage)
                return;
            
            Battery -= energyUsage;
        }

        public void ApplyDamage(float damage)
        {
            if(droneBuilderController.isShieldActive)
                return;
            
            var healthPercentage = 1f - armorPercentage;

            var armorDamage = Mathf.RoundToInt(damage * armorPercentage);
            var healthDamage = Mathf.RoundToInt(damage * healthPercentage);

            var remainingArmor = (int)(Armor - armorDamage);

            if (remainingArmor < 0)
            {
                healthDamage += Mathf.Abs(remainingArmor);
                remainingArmor = 0;
            }

            Armor = remainingArmor;
            Health -= healthDamage;
        }

        public int GetCurrentHealthIndex(float value)
        {
            if (value <= 0) return -1;

            for (var i = 0; i < _thresholds.Length; i++)
                if (value <= _thresholds[i])
                    return _thresholds.Length - i - 1;

            return _thresholds.Length - 1;
        }

        private void OnCollisionEnter(Collision other)
        {
            var trackObject = other.transform.GetComponentInParent<TrackObject>();
            
            EffectsManager.Instance.GetCollisionEffect(other.contacts[0].point, droneBuilderController.currentPercentSpeed);
            SoundManager.Instance.GetSound(other.contacts[0].point, SoundManager.Instance.collisionClip, droneBuilderController.currentPercentSpeed / 100);

            if (trackObject && droneBuilderController.currentPercentSpeed >= 50f)
            {
                switch (trackObject.effectType)
                {
                    case EffectType.Massive:
                        ApplyDamage(droneBuilderController.currentPercentSpeed / 2);
                        break;
                    case EffectType.Destructible:
                        SoundManager.Instance.GetSound(trackObject.transform.position, SoundManager.Instance.explosionClip);
                        CreateExplosion(trackObject, other);
                        break;
                    case EffectType.Hybrid:
                        if (trackObject.objectType == ObjectsType.Lamp)
                        {
                            var lamp = trackObject.GetComponent<Lamp>();
                            if (lamp && lamp.isLampTurn)
                            {
                                SoundManager.Instance.GetSound(trackObject.transform.position, SoundManager.Instance.lampOffClip);
                                CreateLampBrokenEffect(trackObject, other);
                                lamp.TurnLamp();
                            }
                            else
                            {
                                SoundManager.Instance.GetSound(trackObject.transform.position, SoundManager.Instance.explosionClip);
                                CreateExplosion(trackObject, other);
                            }
                            return;
                        }
                        
                        ApplyDamage(trackObject.damage);
                        SoundManager.Instance.GetSound(trackObject.transform.position, SoundManager.Instance.explosionClip);
                        CreateExplosion(trackObject, other);

                        break;
                }
            }
        }

        private void CreateLampBrokenEffect(TrackObject trackObject, Collision other)
        {
            var childRenderersWithCollider = other.gameObject.GetComponentsInChildren<Renderer>()
                .Where(rend => rend.GetComponent<Collider>() != null);
            
            foreach (var renderer in childRenderersWithCollider)
            {
                Vector3 center = renderer.bounds.center;
                EffectsManager.Instance.GetBrokenLampEffect(center, trackObject.Scale);
            }
        }

        private void CreateExplosion(TrackObject trackObject, Collision other)
        {
            EffectsManager.Instance.GetExplosionEffect(other.transform.position, trackObject.Scale);
            if (BuilderManager.Instance.isGameMode)
                Destroy(trackObject.gameObject);
            else
                trackObject.gameObject.SetActive(false);
        }
    }
}