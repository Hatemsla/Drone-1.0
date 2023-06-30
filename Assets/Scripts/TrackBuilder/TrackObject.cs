using Drone;
using UnityEngine;

namespace Builder
{
    public class TrackObject : MonoBehaviour
    {
        public int id;
        public Transform connection;
        public ObjectsType objectType = ObjectsType.Other;
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

        public Vector3 Scale
        {
            get => transform.localScale;
            set => transform.localScale = value;
        }

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        public Quaternion Rotation
        {
            get => transform.rotation;
            set => transform.rotation = value;
        }
    }
}