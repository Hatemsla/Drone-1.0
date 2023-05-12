using System;
using System.Collections.Generic;
using cakeslice;
using Unity.XR.OpenVR;
using UnityEngine;

namespace Builder
{
    public class CreateRope : MonoBehaviour
    {
        public int ropeLength;
        public float ropeOffset;
        public float maxMass;
        public float minMass;
        public GameObject ropePrefab;

        private List<Rigidbody> _ropes = new List<Rigidbody>();

        private void Start()
        {
            float massStep = (maxMass - minMass) / ropeLength;
            float currentMass = maxMass;
            
            for (int i = 0; i < ropeLength; i++)
            {
                var rope = Instantiate(ropePrefab, transform.position + Vector3.down * ropeOffset * i, Quaternion.identity);
                rope.transform.SetParent(transform);
                rope.layer = gameObject.layer;
                var ropeHinge = rope.GetComponent<HingeJoint>();
                var ropeRb = rope.GetComponent<Rigidbody>();
                ropeRb.mass = currentMass;

                if (i == 0)
                    ropeRb.isKinematic = true;

                if (_ropes.Count > 0)
                {
                    ropeHinge.connectedBody = _ropes[^1];
                }

                _ropes.Add(ropeRb);
                currentMass -= massStep;
            }

            TrackBuilderUtils.OffOutlineRecursively(transform);
        }
    }
}