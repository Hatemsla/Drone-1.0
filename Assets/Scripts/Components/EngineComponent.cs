using System;
using UnityEngine;

namespace Components
{
    [Serializable]
    public struct EngineComponent
    {
        public float propellerRotSpeed;
        public Transform[] propellersTransforms;
    }
}