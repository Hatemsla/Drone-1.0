using UnityEngine;

namespace DroneFootball
{
    public abstract class DroneAI : MonoBehaviour
    {
        public float speed;
        public float minMaxPitch;
        public float minMaxRoll;
        public float proportionalGain;
        public float integralGain;
        public float derivativeGain;
        public float outputMin = -1;
        public float outputMax = 1;
        public float integralSaturation;
        public float throttle;
        public float lerpSpeed;
        public Transform targetTransform;
    }
}