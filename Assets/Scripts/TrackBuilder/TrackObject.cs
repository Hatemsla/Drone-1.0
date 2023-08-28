using System;
using System.Collections.Generic;
using System.Linq;
using Drone;
using UnityEngine;

namespace Drone.Builder
{
    public class TrackObject : MonoBehaviour
    {
        public int id;
        public Transform connection;
        public ObjectsType objectType;
        public HasPortal hasPortal = HasPortal.No;
        public InteractiveType interactiveType = InteractiveType.None;
        public EffectType effectType = EffectType.Massive;
        public float yOffset;
        public float yOffsetIncrease;
        public bool isActive;
        public float maxMouseDistance = 3f;
        public float damage = 1f;
        public string objectName;
        public string objectDescription;
        public InteractiveObject interactiveObject;
        
        public event Action<Vector3> ScaleChanged;
        public event Action<Vector3> PositionChanged;
        public event Action<Quaternion> RotationChanged;
        
        public Vector3 Scale
        {
            get => transform.localScale;
            set
            {
                transform.localScale = value;
                ScaleChanged?.Invoke(value);
            }
        }

        public Vector3 Position
        {
            get => transform.position;
            set
            {
                transform.position = value;
                PositionChanged?.Invoke(value);
            }
        }

        public Quaternion Rotation
        {
            get => transform.rotation;
            set
            {
                transform.rotation = value;
                RotationChanged?.Invoke(value);
            }
        }
    }
}