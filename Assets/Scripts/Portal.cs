using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Camera portalView;
    private Portal _otherPortal;

    private void Start()
    {
        var otherPortals = FindObjectsOfType<Portal>();
        foreach (var otherPortal in otherPortals)
        {
            if (otherPortal != this)
                _otherPortal = otherPortal;
        }

        if (!_otherPortal) return;

        _otherPortal.portalView.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        GetComponentInChildren<MeshRenderer>().sharedMaterial.mainTexture = _otherPortal.portalView.targetTexture;
    }

    private void Update()
    {
        if (!_otherPortal) return;
        
        var lookerPosition =
            _otherPortal.transform.worldToLocalMatrix.MultiplyPoint3x4(Camera.main.transform.position);
        lookerPosition = new Vector3(-lookerPosition.x, lookerPosition.y, -lookerPosition.z);
        portalView.transform.localPosition = lookerPosition;

        var difference = transform.rotation *
                         Quaternion.Inverse(_otherPortal.transform.rotation * Quaternion.Euler(0f, 180f, 0f));
        portalView.transform.rotation = difference * Camera.main.transform.rotation;

        portalView.nearClipPlane = lookerPosition.magnitude;
    }
}