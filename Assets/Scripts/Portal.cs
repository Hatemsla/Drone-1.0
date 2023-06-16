using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Camera portalView;
    public Portal otherPortal;

    private void Start()
    {
        var otherPortals = FindObjectsOfType<Portal>();
        foreach (var portal in otherPortals)
        {
            if (portal != this)
                otherPortal = portal;
        }

        if (!otherPortal) return;

        otherPortal.portalView.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        GetComponentInChildren<MeshRenderer>().sharedMaterial.mainTexture = otherPortal.portalView.targetTexture;
    }

    private void Update()
    {
        if (!otherPortal) return;
        
        var lookerPosition =
            otherPortal.transform.worldToLocalMatrix.MultiplyPoint3x4(Camera.main.transform.position);
        lookerPosition = new Vector3(-lookerPosition.x, lookerPosition.y, -lookerPosition.z);
        portalView.transform.localPosition = lookerPosition;

        var difference = transform.rotation *
                         Quaternion.Inverse(otherPortal.transform.rotation * Quaternion.Euler(0f, 180f, 0f));
        portalView.transform.rotation = difference * Camera.main.transform.rotation;

        portalView.nearClipPlane = lookerPosition.magnitude;
    }
}