using System;
using System.Collections.Generic;
using Builder;
using UnityEngine;

namespace Drone
{
    public class Lamp : InteractiveObject
    {
        [SerializeField] private Light lamp;
        [SerializeField] private MeshRenderer[] meshes;

        private Dictionary<Material, bool> _emissions = new Dictionary<Material, bool>();

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

        public void TurnLamp()
        {
            isLampTurn = !isLampTurn;
            lamp.enabled = isLampTurn;

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
    }
}