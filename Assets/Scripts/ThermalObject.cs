using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DroneFootball
{
    public class ThermalObject : MonoBehaviour
    {
        public List<Material> objectMaterials;
        public List<Color> defaultColors;

        private void Start()
        {
            foreach (var material in GetComponent<MeshRenderer>().materials)
            {
                defaultColors.Add(material.color);
            }
            objectMaterials = GetComponent<MeshRenderer>().materials.ToList();
        }
    }
}