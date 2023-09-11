using System;
using System.Collections.Generic;
using Drone.Builder;
using UnityEngine;

namespace Drone.Builder
{
    public class Lamp : InteractiveObject
    {
        public bool isLampTurn = true;
        [SerializeField] private Light lamp;
        [SerializeField] private MeshRenderer[] meshes;
        public GameObject lampObject;

        private readonly Dictionary<Material, bool> _emissions = new();

        private void Start()
        {
            foreach (var mesh in meshes)
            {
                foreach (var mat in mesh.materials)
                {
                    _emissions.Add(mat, mat.IsKeywordEnabled("_EMISSION"));
                }
            }
            
            foreach (var mesh in meshes)
            {
                foreach (var mat in mesh.materials)
                {
                    if(isLampTurn && _emissions.TryGetValue(mat, out var isEmissionEnabled) && isEmissionEnabled)
                        mat.EnableKeyword("_EMISSION");
                    else
                        mat.DisableKeyword("_EMISSION");
                }
            }
        }

        public void TurnOn()
        {
            isLampTurn = true;
            lamp.enabled = isLampTurn;
            
            foreach (var mesh in meshes)
            {
                foreach (var mat in mesh.materials)
                {
                    if (_emissions.TryGetValue(mat, out var isEmissionEnabled) && isEmissionEnabled)
                    {
                        mat.EnableKeyword("_EMISSION");
                    }
                }
            }
        }
        
        public void TurnOff()
        {
            isLampTurn = false;
            lamp.enabled = isLampTurn;
            
            foreach (var mesh in meshes)
            {
                foreach (var mat in mesh.materials)
                {
                    if (_emissions.TryGetValue(mat, out var isEmissionEnabled) && isEmissionEnabled)
                    {
                        mat.DisableKeyword("_EMISSION");
                    }
                }
            }
        }

        public void TurnLamp()
        {
            isLampTurn = !isLampTurn;
            lamp.enabled = isLampTurn;

            foreach (var mesh in meshes)
            {
                foreach (var mat in mesh.materials)
                {
                    if (isLampTurn && _emissions.TryGetValue(mat, out var isEmissionEnabled) && isEmissionEnabled)
                        mat.EnableKeyword("_EMISSION");
                    else
                        mat.DisableKeyword("_EMISSION");
                }
            }
        }

        private void SetColor(Color newColor)
        {
            lamp.color = newColor;
            foreach (var mesh in meshes)
            {
                foreach (var mat in mesh.materials)
                {
                    if (_emissions.TryGetValue(mat, out var isEmissionEnabled) && isEmissionEnabled)
                    {
                        mat.SetColor("_EmissionColor", newColor);
                        mat.SetColor("_Color", newColor);
                    }
                }
            }            
        }

        public override void SetActive(bool active)
        {
            isActive = active;
            if(isActive)
                TurnOn();
            else
                TurnOff();
        }

        public override void SetColorIndex(int value)
        {
            colorIndex = value;
            SetColor(GetColorFromOption((ColorOption)value));
        }
    }
}