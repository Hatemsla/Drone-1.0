using UnityEngine;

namespace DroneFootball
{
    public abstract class DroneController : MonoBehaviour
    {
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
    }
}