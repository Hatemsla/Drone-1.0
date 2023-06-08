using UnityEngine;

namespace DroneFootball
{
    [CreateAssetMenu]
    public class GameData : ScriptableObject
    {
        public string levelName;
        public bool isSimpleMode;
        public int currentDifficultIndex;
        public int currentControlDifficultIndex;
        public int currentResolutionIndex;
        public float currentYawSensitivity = 1;
        public float currentEffectsVolume;
        public float currentMusicsVolume;
        public int gameTimeInSeconds;
        public float currentAIDroneSpeed;
        public float currentGateScale;
        public bool isLoadLevel;
        public bool isStartBuilder;
    }
}