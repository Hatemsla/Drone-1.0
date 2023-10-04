using UnityEngine;

namespace Drone
{
    [CreateAssetMenu]
    public class GameData : ScriptableObject
    {
        public string originLevel;
        public string currentLevel;
        public bool isSimpleMode;
        public int currentDifficultIndex;
        public int currentControlDifficultIndex;
        public int currentResolutionIndex;
        public int currentSpeedDownRateIndex;
        public float currentYawSensitivity = 1;
        public float currentEffectsVolume;
        public float currentMusicsVolume;
        public int raceTimeInSeconds;
        public int footballTimeInSeconds;
        public int builderTimeInSeconds;
        public float currentAIDroneSpeed;
        public float currentGateScale;
        public bool isLoadLevel;
        public bool isStartBuilder;
        public bool isTeleportLevel;
        public bool isErrorLoad;

        public void Reset()
        {
            isErrorLoad = false;
            isLoadLevel = false;
            isSimpleMode = true;
            isStartBuilder = false;
            isTeleportLevel = false;
        }
    }
}