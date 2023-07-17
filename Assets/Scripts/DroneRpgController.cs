using Builder;
using UnityEngine;

namespace Drone
{
    public class DroneRpgController : MonoBehaviour
    {
        public DroneController droneBuilderController;
        public DroneData DroneData;
        public int powerUsageRate;
        public bool isAlive = true;
        public bool isCharged = true;
        [SerializeField] private float armorPercentage = 0.9f;
        [SerializeField] private ParticleSystem explosionPrefab;

        private readonly float[] _thresholds = { 0, 12.5f, 25, 37.5f, 50, 62.5f, 75, 87.5f, 100 };

        private void Start()
        {
            DroneData = new DroneData(100, 100, 100);
        }

        private void Update()
        {
            if (BuilderManager.Instance.isMove && !RewindManager.Instance.IsBeingRewinded)
                ApplyEnergyUsage(powerUsageRate * Time.deltaTime);

            if (DroneData.Health <= 0) isAlive = false;

            if (DroneData.Battery <= 0) isCharged = false;
        }

        public void ApplyEnergyUsage(float energyUsage)
        {
            DroneData.Battery -= energyUsage;
        }

        public void ApplyDamage(float damage)
        {
            if(droneBuilderController.isShieldActive)
                return;
            
            var healthPercentage = 1f - armorPercentage;

            var armorDamage = Mathf.RoundToInt(damage * armorPercentage);
            var healthDamage = Mathf.RoundToInt(damage * healthPercentage);

            var remainingArmor = (int)(DroneData.Armor - armorDamage);

            if (remainingArmor < 0)
            {
                healthDamage += Mathf.Abs(remainingArmor);
                remainingArmor = 0;
            }

            DroneData.Armor = remainingArmor;
            DroneData.Health -= healthDamage;
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
            var explosion = Instantiate(explosionPrefab, other.transform.position, Quaternion.identity);
            explosion.transform.localScale = new Vector3(trackObject.Scale.x, trackObject.Scale.y, trackObject.Scale.z);
            if (BuilderManager.Instance.isGameMode)
                Destroy(trackObject.gameObject);
            else
                trackObject.gameObject.SetActive(false);
        }
    }
}