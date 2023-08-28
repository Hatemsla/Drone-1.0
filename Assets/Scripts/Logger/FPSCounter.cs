using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Drone
{
    public class FPSCounter : MonoBehaviour
    {

        private float _deltaTime;

        private void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = -1;
        }

        private void Update()
        {
            _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
        }

//         private void OnGUI()
//         {
// #if DEVELOPMENT_BUILD
//             
//             int w = Screen.width, h = Screen.height;
//
//             GUIStyle style = new GUIStyle();
//
//             Rect rect = new Rect(0, 0, w, h * 2 / 100);
//             style.alignment = TextAnchor.UpperLeft;
//             style.fontSize = h * 2 / 100;
//             style.normal.textColor = new Color(1f, 1f, 1f, 1.0f);
//
//             float msec = _deltaTime * 1000.0f;
//             float fps = 1.0f / _deltaTime;
//             string text = $"{msec:0.0} ms ({fps:0.} fps)";
//             GUI.Label(rect, text, style);
// #endif
//         }
    }
}
