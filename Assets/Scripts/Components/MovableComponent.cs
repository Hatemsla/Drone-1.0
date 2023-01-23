using System;
using UnityEngine;

namespace Components
{
    [Serializable]
    public struct MovableComponent
    {
        public Rigidbody rb;
        public float minMaxPitch;
        public float minMaxRoll;
        public float yawPower;
        public float lerpSpeed;
        public float maxPower;
    }
}