using System;
using System.Collections.Generic;
using UnityEngine;

namespace Drone
{
    public class Lamp : MonoBehaviour
    {
        public Light lamp;
        public MeshRenderer[] meshes;
        public bool isTurn = true;

        private Dictionary<Material, bool> _emissions = new Dictionary<Material, bool>();

        private void Start()
        {
            for (var i = 0; i < meshes.Length; i++)
            {
                for (var j = 0; j < meshes[i].materials.Length; j++)
                {
                    if (meshes[i].materials[j].IsKeywordEnabled("_EMISSION"))
                    {
                        _emissions.Add(meshes[i].materials[j], true);
                    }
                    else
                    {
                        _emissions.Add(meshes[i].materials[j], false);
                    }
                }
            }
        }

        public void TurnOn()
        {
            isTurn = true;
            lamp.enabled = isTurn;
            
            for (int i = 0; i < meshes.Length; i++)
            {
                for (int j = 0; j < meshes[i].materials.Length; j++)
                {
                    if (_emissions.TryGetValue(meshes[i].materials[j], out var isEmissionEnabled) && isEmissionEnabled)
                    {
                        meshes[i].materials[j].EnableKeyword("_EMISSION");
                    }
                }
            }
        }

        public void TurnLamp()
        {
            isTurn = !isTurn;
            lamp.enabled = isTurn;

            for (int i = 0; i < meshes.Length; i++)
            {
                for (int j = 0; j < meshes[i].materials.Length; j++)
                {
                    if(isTurn && _emissions.TryGetValue(meshes[i].materials[j], out var isEmissionEnabled) && isEmissionEnabled)
                        meshes[i].materials[j].EnableKeyword("_EMISSION");
                    else
                        meshes[i].materials[j].DisableKeyword("_EMISSION");
                }
            }
        }
    }
}