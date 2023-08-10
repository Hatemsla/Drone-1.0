using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Drone.Builder
{
    public class ThermalObject : MonoBehaviour
    {
        public List<Material> objectMaterials;
        public List<Color> defaultColors;
        public List<bool> hasEmissions;

        private void Start()
        {
            foreach (var material in GetComponent<MeshRenderer>().materials)
            {
                defaultColors.Add(material.color);
                hasEmissions.Add(material.IsKeywordEnabled("_EMISSION"));
            }
            objectMaterials = GetComponent<MeshRenderer>().materials.ToList();
        }
    }
}