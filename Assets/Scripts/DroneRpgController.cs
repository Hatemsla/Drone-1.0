using System;
using System.Collections.Generic;
using Builder;
using UnityEngine;

namespace Drone
{
    public class DroneRpgController : MonoBehaviour
    {
        public DroneController droneBuilderController;
        [SerializeField] private DroneData droneData;
        public float powerUsageRate;
        public bool isAlive = true;
        public bool isCharged = true;
        public bool isEnergyUsage = true;
        [SerializeField] private float armorPercentage = 0.9f;

        public event Action SkillsCountChangedEvent;

        private readonly float[] _thresholds = { 0, 20f, 40f, 60f, 80f, 100f };

        public Dictionary<Skills, int> SkillsCount
        {
            get => droneData.skillsCount;
            set
            {
                droneData.skillsCount = value;
                SkillsCountChangedEvent?.Invoke();
            }
        }

        public float Battery
        {
            get => droneData.Battery;
            set
            {
                droneData.Battery = value;
                if (value <= 0)
                    isCharged = false;
            }
        }

        public float Health
        {
            get => droneData.Health;
            set
            {
                droneData.Health = value;

                if (droneData.Health > 100)
                    droneData.Health = 100;
                
                if (value <= 0)
                    isAlive = false;
            }
        }

        public float Armor
        {
            get => droneData.Armor;
            set
            {
                droneData.Armor = value;

                if (droneData.Armor > 100)
                    droneData.Armor = 100;
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

        public void UpdateSkillValue(Skills skill, int newValue)
        {
            SkillsCount[skill] = newValue;
            SkillsCountChangedEvent?.Invoke();
        }

        private void Update()
        {
            if (BuilderManager.Instance.isMove && !RewindManager.Instance.IsBeingRewinded)
                ApplyEnergyUsage(powerUsageRate * Time.deltaTime);
        }

        public void ResetDroneData()
        {
            droneData.Battery = 100;
            droneData.Health = 100;
            droneData.Armor = 100;
            droneData.Coins = 0;
            droneData.Crystals = 0;
        }

        public void ApplyEnergyUsage(float energyUsage)
        {
            if(!isEnergyUsage)
                return;
            
            droneData.Battery -= energyUsage;
        }

        public void ApplyDamage(float damage)
        {
            if(droneBuilderController.isShieldActive)
                return;
            
            var healthPercentage = 1f - armorPercentage;

            var armorDamage = Mathf.RoundToInt(damage * armorPercentage);
            var healthDamage = Mathf.RoundToInt(damage * healthPercentage);

            var remainingArmor = (int)(droneData.Armor - armorDamage);

            if (remainingArmor < 0)
            {
                healthDamage += Mathf.Abs(remainingArmor);
                remainingArmor = 0;
            }

            droneData.Armor = remainingArmor;
            droneData.Health -= healthDamage;
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
            
            EffectsManager.Intsance.GetSparksEffect(other.contacts[0].point, droneBuilderController.currentPercentSpeed);

            if (trackObject && droneBuilderController.currentPercentSpeed >= 50f)
            {
                switch (trackObject.effectType)
                {
                    case EffectType.Massive:
                        ApplyDamage(droneBuilderController.currentPercentSpeed / 2);
                        break;
                    case EffectType.Destructible:
                        CreateExplosion(trackObject, other);
                        break;
                    case EffectType.Hybrid:
                        if (trackObject.objectType == ObjectsType.Lamp)
                        {
                            var lamp = trackObject.GetComponent<Lamp>();
                            if (lamp && lamp.isLampTurn)
                            {
                                lamp.TurnLamp();
                            }
                            else
                            {
                                CreateExplosion(trackObject, other);
                            }
                            return;
                        }
                        
                        ApplyDamage(trackObject.damage);
                        CreateExplosion(trackObject, other);

                        break;
                }
            }
        }

        private void CreateExplosion(TrackObject trackObject, Collision other)
        {
            EffectsManager.Intsance.GetExplosionEffect(other.transform.position, trackObject.Scale);
            if (BuilderManager.Instance.isGameMode)
                Destroy(trackObject.gameObject);
            else
                trackObject.gameObject.SetActive(false);
        }
    }
}