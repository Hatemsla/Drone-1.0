using UnityEngine;

namespace Drone
{
    public abstract class DroneController : MonoBehaviour
    {
        protected const float MaxVelocity = 13f;
        public float targetSpeed;
        public float minMaxPitch;
        public float minMaxRoll;
        public float yawPower;
        public Vector2 cyclic;
        public float pedals;
        public float throttle;
        public float lerpSpeed;
        public float yaw;
        public bool isSimpleMode;
        public bool isStop;
        public float currentPercentSpeed;
        public float currentSpeed;
        public int boostsCount;
        public bool isShieldActive;
    }
}