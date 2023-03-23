using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera camera;
    public Transform[] camerasPositions;

    private void Start()
    {
        camera.transform.localPosition = camerasPositions[0].localPosition;
        camera.nearClipPlane = 0.3f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            camera.transform.localPosition = camerasPositions[0].localPosition;
            camera.nearClipPlane = 0.3f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            camera.transform.localPosition = camerasPositions[1].localPosition;
            camera.nearClipPlane = 0.05f;
        }
    }
}
