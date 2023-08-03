using System.Collections.Generic;

namespace Builder
{
    class HealingStation
    {
        private readonly Dictionary<int, float> _healingTimeByHpThreshold = new Dictionary<int, float>
        {
            { 180, 4.0f },
            { 160, 5.1f },
            { 140, 6.2f },
            { 120, 7.3f },
            { 100, 8.4f },
            { 80, 9.5f },
            { 60, 10.6f },
            { 40, 11.7f },
            { 20, 12.8f },
            { 0, 15f }
        };
        
        public float GetHealingTime(float currentHp)
        {
            foreach (var threshold in _healingTimeByHpThreshold)
            {
                if (currentHp > threshold.Key)
                    return threshold.Value;
            }
            
            return _healingTimeByHpThreshold[0];
        }
    }
}