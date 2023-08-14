using System.Collections.Generic;
using Drone.Builder;
using UnityEngine;

namespace Drone
{
    [CreateAssetMenu]
    public class DroneData : ScriptableObject
    {
        [SerializeField] private float health;
        [SerializeField] private float armor;
        [SerializeField] private float battery;
        [SerializeField] private int coins;
        [SerializeField] private int crystals;
        
        public Dictionary<Skills, int> skillsCount = new Dictionary<Skills, int>()
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