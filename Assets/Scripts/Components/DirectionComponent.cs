using System;
using UnityEngine;

namespace Components
{
    [Serializable]
    public struct DirectionComponent
    {
        public Vector2 cyclic;
        public float pedals;
        public float throttle;
    }
}