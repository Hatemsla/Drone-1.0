﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Drone.Builder
{
    public class ThermalVision : MonoBehaviour
    {
        [SerializeField] private DroneRpgController droneRpgController;
        [SerializeField] private Color thermalColor;
        [SerializeField] private float darkeningAmount = 0.5f;
        [SerializeField] private Shader xRayShader;
        [SerializeField] private Color xRayColor;
        [SerializeField] private ThermalObject[] thermalObjects;
        [SerializeField] private List<Camera> cameras;
        [SerializeField] private bool isThermalVision;
        [SerializeField] private bool isXRay;

        private void Start()
        {
            cameras = FindObjectsOfType<Camera>().ToList();
            BuilderManager.Instance.TestLevelEvent += OnTurnOffThermalVision;
            InputManager.Instance.ThermalVisionEvent += OnThermalVision;
            InputManager.Instance.XRayEvent += OnXRay;
        }

        private void OnDisable()
        {
            BuilderManager.Instance.TestLevelEvent -= OnTurnOffThermalVision;
            InputManager.Instance.ThermalVisionEvent -= OnThermalVision;
            InputManager.Instance.XRayEvent -= OnXRay;
        }

        private void OnXRay()
        {
            if(!BuilderManager.Instance.isMove || droneRpgController.SkillsCount[Skills.XRay] <= 0)
                return;
            
            OnTurnOffThermalVision();
            isXRay = !isXRay;
            TurnXRay();
        }

        private void OnThermalVision()
        {
            if(!BuilderManager.Instance.isMove || droneRpgController.SkillsCount[Skills.Thermal] <= 0)
                return;

            isXRay = false;
            TurnXRay();
            isThermalVision = !isThermalVision;
            foreach (var cam in cameras)
            {
                cam.clearFlags = isThermalVision ? CameraClearFlags.SolidColor : CameraClearFlags.Skybox;
            }

            TurnThermalVision();
            SetDarkeningForNonThermalObjects();
        }

        private void TurnXRay()
        {
            if (isXRay)
            {
                foreach (var cam in cameras)
                    cam.SetReplacementShader(xRayShader, "");
                Shader.SetGlobalColor("_OverDrawColor", xRayColor);
            }
            else
            {
                foreach (var cam in cameras)
                    cam.ResetReplacementShader();
            }
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