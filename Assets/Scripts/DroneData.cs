using System.Collections.Generic;
using Drone.Builder;
using UnityEngine;

namespace Drone
{
    [CreateAssetMenu]
    public class DroneData : ScriptableObject
    {
        [SerializeField] private bool isReset;
        [SerializeField] private float health;
        [SerializeField] private float armor;
        [SerializeField] private float battery;
        [SerializeField] private int coins;
        [SerializeField] private int crystals;
        [SerializeField] private float timeForEndGame;
        
        public Dictionary<Skills, int> skillsCount = new()
        {
            { Skills.Shield, 0 },
            { Skills.HpRestore, 0 },
            { Skills.ArmorRestore, 0 },
            { Skills.Jerk, 0 },
            { Skills.XRay, 0 },
            { Skills.Thermal, 0 },
            { Skills.TimeRewind, 0 },
            { Skills.Flashlight, 0 },
        };
        
        public Dictionary<Skills, int[]> skillsCosts = new()
        {
            { Skills.None,         new[] { 0, 0, 0 } },
            { Skills.Shield,       new[] { 30, 15, 2 } },
            { Skills.HpRestore,    new[] { 15, 7, 1 } },
            { Skills.ArmorRestore, new[] { 15, 7, 1 } },
            { Skills.Jerk,         new[] { 20, 10, 1 } },
            { Skills.XRay,         new[] { 60, 25, 4 } },
            { Skills.Thermal,      new[] { 50, 20, 3 } },
            { Skills.TimeRewind,   new[] { 120, 50, 10 } },
            { Skills.Flashlight,   new[] { 40, 17, 2 } },
            { Skills.FlashlightUV, new[] { 60, 25, 3 } },
        };

        public Dictionary<Skills, List<MoneyType>> buyingTypes = new()
        {
            { Skills.Shield,       new List<MoneyType>() },
            { Skills.HpRestore,    new List<MoneyType>() },
            { Skills.ArmorRestore, new List<MoneyType>() },
            { Skills.Jerk,         new List<MoneyType>() },
            { Skills.XRay,         new List<MoneyType>() },
            { Skills.Thermal,      new List<MoneyType>() },
            { Skills.TimeRewind,   new List<MoneyType>() },
            { Skills.Flashlight,   new List<MoneyType>() },
            { Skills.FlashlightUV, new List<MoneyType>() },
        };

        public float TimeForEndGame
        {
            get => timeForEndGame;
            set => timeForEndGame = value;
        }
        
        public bool IsReset
        {
            get => isReset;
            set => isReset = value;
        }

        public int Crystals
        {
            get => crystals;
            set => crystals = value;
        }
        
        public int Coins
        {
            get => coins;
            set => coins = value;
        }
        
        public float Health
        {
            get => health;
            set
            {
                if (value > 100)
                    health = 100;
                else
                    health = value;
            }
        }

        public float Armor
        {
            get => armor;
            set
            {
                if (value > 100)
                    armor = 100;
                else
                    armor = value;
            }
        }
        
        public float Battery
        {
            get => battery;
            set
            {
                if (value > 100)
                    battery = 100;
                else
                    battery = value;
            }
        }
    }
}