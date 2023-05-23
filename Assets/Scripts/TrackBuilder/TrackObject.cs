using System;
using System.Collections;
using System.Collections.Generic;
using Builder;
using UnityEngine;

namespace Builder
{
    public class TrackObject : MonoBehaviour
    {
        public int id;
        public ObjectsType objectType;
        public float yOffset;
        public bool isActive;
        public float maxMouseDistance = 3f;
        public Vector3 Scale => transform.localScale;
        public Vector3 Position => transform.localPosition;
        public Quaternion Rotation => transform.localRotation;
    }
}