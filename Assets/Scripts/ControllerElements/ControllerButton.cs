using System.Collections;
using System.Collections.Generic;
using Drone;
using UnityEngine;

namespace Drone.Builder.ControllerElements
{
    public class ControllerButton : InteractiveObject
    {
        public GameObject controllerButtonObject;
        public GameObject controllerButtonObject2;

        public ColorOption selectedColorOption;
        public float glowIntensity = 1f;
        public float TriggerExitDelay = 2f;

        private Renderer objectRenderer;
        private Renderer objectRenderer2;
        private Material originalMaterial;
        private Material originalMaterial2;
        private Material glowMaterial; 
        private Material glowMaterial2; 

        private bool isGlowing = false;
        private bool isChanged = true;

        void Start()
        {
            objectRenderer = controllerButtonObject.GetComponent<Renderer>();
            objectRenderer2 = controllerButtonObject2.GetComponent<Renderer>();
            SetColor(TrackBuilderUtils.GetColorFromOption(selectedColorOption), false);       
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<GrabbedObject>() || other.GetComponentInParent<DroneController>())
            // if (other.CompareTag("Block") || other.CompareTag("Player"))
            {
                SetColor(TrackBuilderUtils.GetColorFromOption(selectedColorOption), true);
                setColorActiv(selectedColorOption, true);
                isChanged = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponentInParent<GrabbedObject>() || other.GetComponentInParent<DroneController>())
            // if (other.CompareTag("Block") || other.CompareTag("Player")) 
            {
                StartCoroutine(ChangeVariableAfterDelay(TriggerExitDelay));
            }
        }

        private System.Collections.IEnumerator ChangeVariableAfterDelay(float delay)
        {
            isChanged = false;
            yield return new WaitForSeconds(delay);
            Debug.Log("isChanged");

            Debug.Log(isChanged);
            if (!isChanged)
            {
                SetColor(TrackBuilderUtils.GetColorFromOption(selectedColorOption), false);
                setColorActiv(selectedColorOption, false);
                isChanged = true;
            }
        }

        private void SetColor(Color newColor, bool isGlowing)
        {
            this.isGlowing = isGlowing;
            if (isGlowing)
            {
                objectRenderer.materials[1].SetColor("_Color", newColor);
                objectRenderer2.materials[1].SetColor("_Color", newColor);
                objectRenderer.materials[1].EnableKeyword("_EMISSION");
                objectRenderer2.materials[1].EnableKeyword("_EMISSION");
                objectRenderer.materials[1].SetColor("_EmissionColor", newColor * glowIntensity);
                objectRenderer2.materials[1].SetColor("_EmissionColor", newColor * glowIntensity);
            }
            else
            {
                objectRenderer.materials[1].SetColor("_Color", newColor);
                objectRenderer2.materials[1].SetColor("_Color", newColor);
                objectRenderer.materials[1].DisableKeyword("_EMISSION");
                objectRenderer2.materials[1].DisableKeyword("_EMISSION");   
              
            }
        }

        private void setColorActiv(ColorOption option, bool isActiv)
        {
            if (option == ColorOption.Красный)
            {
                BuilderManager.Instance.isActivRed = isActiv;
            }
            else if (option == ColorOption.Зелёный)
            {
                BuilderManager.Instance.isActivGreen = isActiv;
            }
            else if (option == ColorOption.Жёлтый)
            {
                BuilderManager.Instance.isActivYellow = isActiv;
            }
            else if (option == ColorOption.Синий)
            {
                BuilderManager.Instance.isActivBlue  = isActiv;
            }
        }

        public void set_color_index(int value)
        {
            color_index = value;
            selectedColorOption = (ColorOption)value;
            SetColor(TrackBuilderUtils.GetColorFromOption(selectedColorOption), false);        
        }

        public override void SetColorIndex(int value)
        {
            color_index = value;
            selectedColorOption = (ColorOption)value;
            SetColor(TrackBuilderUtils.GetColorFromOption(selectedColorOption), false);
        }

        public void set_time_value(float value)
        {
            buttonDelay = value;
            TriggerExitDelay = value;

            Debug.Log("set_time_value");
            Debug.Log(value);
            // TriggerExitDelay = value;
        }

        public override void SetActive(bool active)
        {
            
        }
    }
}
