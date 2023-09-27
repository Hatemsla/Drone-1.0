using System;
using UnityEngine;

namespace Drone.Builder
{
    [Serializable]
    public struct ConnectionPosition
    {
        public ConnectionDirection thisConnectionDirection;
        public ConnectionDirection otherConnectionDirection;
        public Vector3 offset;
        public Vector3 rotation;
    }
}