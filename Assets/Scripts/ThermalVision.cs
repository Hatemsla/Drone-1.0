using System;
using System.Collections.Generic;
using System.Linq;
using Builder;
using UnityEngine;

namespace DroneFootball
{
    public class ThermalVision : MonoBehaviour
    {
        public Color thermalColor;
        public float darkeningAmount = 0.5f;

        public ThermalObject[] thermalObjects;
        public List<Camera> cameras;
        public bool isThermalVision;

        private void Start()
        {
            cameras = FindObjectsOfType<Camera>().ToList();
            BuilderManager.Instance.TestLevelEvent.AddListener(OnTurnOffThermalVision);
        }

        private void OnTurnOffThermalVision()
        {
            if (isThermalVision)
            {
                SetDefaultDarkeningForNonThermalObjects();
                TurnOffThermalVision();
            }

            isThermalVision = false;
            foreach (var cam in cameras)
            {
                cam.clearFlags = CameraClearFlags.Skybox;
            }
        }

        private void TurnOffThermalVision()
        {
            thermalObjects = FindObjectsOfType<ThermalObject>();
            foreach (var obj in thermalObjects)
            {
                SwitchToDefaultColor(obj);
            }
        }

        private void SetDefaultDarkeningForNonThermalObjects()
        {
            var allObjects = FindObjectsOfType<Renderer>();
            foreach (var obj in allObjects)
            {
                if (obj.GetComponent<ThermalObject>()) continue;

                foreach (var objectMaterial in obj.materials)
                {
                    if (objectMaterial.HasProperty("_Color"))
                    {
                        var originalColor = objectMaterial.GetColor("_Color");
                        var darkenedColor = originalColor / darkeningAmount;
                        objectMaterial.SetColor("_Color", darkenedColor);
                    }
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T) && BuilderManager.Instance.isMove)
            {
                isThermalVision = !isThermalVision;
                foreach (var cam in cameras)
                {
                    cam.clearFlags = isThermalVision ? CameraClearFlags.SolidColor : CameraClearFlags.Skybox;
                }
                TurnThermalVision();
                SetDarkeningForNonThermalObjects();
            }
        }

        private void TurnThermalVision()
        {
            if (!isThermalVision)
            {
                foreach (var obj in thermalObjects)
                {
                    SwitchToDefaultColor(obj);
                }
                return;
            }

            thermalObjects = FindObjectsOfType<ThermalObject>();

            foreach (var obj in thermalObjects)
            {
                HighlightObject(obj);
            }
        }

        private void HighlightObject(ThermalObject obj)
        {
            foreach (var objectMaterial in obj.objectMaterials)
            {
                objectMaterial.SetColor("_Color", thermalColor);
                objectMaterial.SetColor("_EmissionColor", thermalColor);
                objectMaterial.EnableKeyword("_EMISSION");
            }
        }

        private void SwitchToDefaultColor(ThermalObject obj)
        {
            for (var i = 0; i < obj.objectMaterials.Count; i++)
            {
                foreach (var defaultColor in obj.defaultColors)
                {
                    obj.objectMaterials[i].SetColor("_Color", obj.defaultColors[i]);
                    obj.objectMaterials[i].SetColor("_EmissionColor", Color.white);
                    if(!obj.hasEmissions[i])
                        obj.objectMaterials[i].DisableKeyword("_EMISSION");
                }
            }
        }
        
        private void SetDarkeningForNonThermalObjects()
        {
            var allObjects = FindObjectsOfType<Renderer>();
            foreach (var obj in allObjects)
            {
                if (obj.GetComponent<ThermalObject>()) continue;
                
                foreach (var objectMaterial in obj.materials)
                {
                    if (objectMaterial.HasProperty("_Color"))
                    {
                        var originalColor = objectMaterial.GetColor("_Color");
                        var darkenedColor = isThermalVision
                            ? originalColor * darkeningAmount
                            : originalColor / darkeningAmount;
                        objectMaterial.SetColor("_Color", darkenedColor);
                    }
                }
            }
        }
    }
}