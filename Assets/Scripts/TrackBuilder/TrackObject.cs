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
        public string objectName; 
        public string objectDescription; 
        
        public Vector3 Scale
        {
            get => transform.localScale;
            set => transform.localScale = value;
        }

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = new Vector3(value.x, value.y + yOffset, value.z);
        }

        public Quaternion Rotation
        {
            get => transform.rotation;
            set => transform.rotation = value;
        }
    }
}