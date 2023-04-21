using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera mainCamera;
    public Transform[] camerasPositions;

    private void Start()
    {
        mainCamera = Camera.main;
        mainCamera.transform.localPosition = camerasPositions[0].localPosition;
        mainCamera.nearClipPlane = 0.3f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            mainCamera.transform.localPosition = camerasPositions[0].localPosition;
            mainCamera.nearClipPlane = 0.3f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            mainCamera.transform.localPosition = camerasPositions[1].localPosition;
            mainCamera.nearClipPlane = 0.05f;
        }
    }
}
