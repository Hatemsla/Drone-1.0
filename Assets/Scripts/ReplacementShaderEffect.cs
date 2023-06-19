using System;
using UnityEngine;

namespace Drone
{
    public class ReplacementShaderEffect : MonoBehaviour
    {
        public Shader replacementShader;
        public Color overDrawColor;
        private bool _isXRay;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                _isXRay = !_isXRay;
                if (_isXRay)
                {
                    GetComponent<Camera>().SetReplacementShader(replacementShader, "");
                    Shader.SetGlobalColor("_OverDrawColor", overDrawColor);
                }
                else
                {
                    GetComponent<Camera>().ResetReplacementShader();   
                }
            }
        }
    }
}