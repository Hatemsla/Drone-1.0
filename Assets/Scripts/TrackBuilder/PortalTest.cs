using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Drone.Builder
{
    public class PortalTest : MonoBehaviour
    {
        public Camera portalView;
        public PortalTest otherPortalTest;

        private void Start()
        {
            var otherPortals = FindObjectsOfType<PortalTest>();
            foreach (var portal in otherPortals)
            {
                if (portal != this)
                    otherPortalTest = portal;
            }

            if (!otherPortalTest) return;

            otherPortalTest.portalView.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
            GetComponentInChildren<MeshRenderer>().sharedMaterial.mainTexture = otherPortalTest.portalView.targetTexture;
        }

        private void Update()
        {
            if (!otherPortalTest) return;
        
            var lookerPosition =
                otherPortalTest.transform.worldToLocalMatrix.MultiplyPoint3x4(Camera.main.transform.position);
            lookerPosition = new Vector3(-lookerPosition.x, lookerPosition.y, -lookerPosition.z);
            portalView.transform.localPosition = lookerPosition;

            var difference = transform.rotation *
                             Quaternion.Inverse(otherPortalTest.transform.rotation * Quaternion.Euler(0f, 180f, 0f));
            portalView.transform.rotation = difference * Camera.main.transform.rotation;

            portalView.nearClipPlane = lookerPosition.magnitude;
        }
    }
}