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

        private readonly float[] _thresholds = { 0, 12.5f, 25, 37.5f, 50, 62.5f, 75, 87.5f, 100 };

        private void Start()
        {
            DroneData = new DroneData(100, 100, 100);
        }

        private void Update()
        {
            if (BuilderManager.Instance.isMove && !RewindManager.Instance.IsBeingRewinded)
                DroneData.Battery -= powerUsageRate * Time.deltaTime;

            if (DroneData.Health <= 0) isAlive = false;

            if (DroneData.Battery <= 0) isCharged = false;
        }

        public void ApplyDamage(float damage)
        {
            var armorPercentage = 0.9f;
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
                switch (trackObject.effectType)
                {
                    case EffectType.Massive:
                        ApplyDamage(droneBuilderController.currentPercentSpeed / 2);
                        break;
                    case EffectType.Destructible:
                        if (BuilderManager.Instance.isGameMode)
                            Destroy(trackObject.gameObject);
                        else
                            transform.gameObject.SetActive(false);
                        break;
                    case EffectType.Hybrid:
                        if (trackObject.objectType == ObjectsType.Lamp)
                        {
                            var lamp = trackObject.GetComponent<Lamp>();
                            if (lamp.isLampTurn)
                            {
                                lamp.TurnLamp();
                            }
                            else
                            {
                                if (BuilderManager.Instance.isGameMode)
                                    Destroy(trackObject.gameObject);
                                else
                                    trackObject.gameObject.SetActive(false);
                            }
                            return;
                        }

                        ApplyDamage(trackObject.damage);
                        if (BuilderManager.Instance.isGameMode)
                            Destroy(trackObject.gameObject);
                        else
                            trackObject.gameObject.SetActive(false);
                        break;
                }
        }
    }
}