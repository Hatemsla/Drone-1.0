using System;
using System.Collections.Generic;
using System.Linq;
using Builder;
using UnityEngine;

namespace DroneFootball
{
    public class ThermalVision : MonoBehaviour
    {
        public Camera mainCamera;
        public Color thermalColor;
        public float darkeningAmount = 0.5f;

        public List<ThermalObject> thermalObjects;
        private bool _isThermalVision;

        private void Start()
        {
            BuilderManager.Instance.TestLevelEvent.AddListener(TurnOffThermalVision);
        }

        private void TurnOffThermalVision()
        {
            _isThermalVision = false;
            mainCamera.clearFlags = _isThermalVision ? CameraClearFlags.SolidColor : CameraClearFlags.Skybox;
            TurnThermalVision();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T) && BuilderManager.Instance.isMove)
            {
                _isThermalVision = !_isThermalVision;
                mainCamera.clearFlags = _isThermalVision ? CameraClearFlags.SolidColor : CameraClearFlags.Skybox;
                TurnThermalVision();
            }
        }

        private void TurnThermalVision()
        {
            if (!_isThermalVision)
            {
                foreach (var obj in thermalObjects)
                {
                    SwitchToDefaultColor(obj);
                }

                SetDarkeningForNonThermalObjects();
                return;   
            }
            
            thermalObjects = FindObjectsOfType<ThermalObject>().ToList();

            foreach (var obj in thermalObjects)
            {
                HighlightObject(obj);
            }

            SetDarkeningForNonThermalObjects();
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
                    var originalColor = objectMaterial.GetColor("_Color");
                    var darkenedColor = _isThermalVision ? originalColor * darkeningAmount : originalColor / darkeningAmount;
                    objectMaterial.SetColor("_Color", darkenedColor);
                }
            }
        }
    }
}