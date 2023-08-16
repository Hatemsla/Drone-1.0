using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Drone;

namespace Drone.Builder.ControllerElements
{
    public class ControllerPanel : InteractiveObject
    {
        public GameObject controllerPanelObject;
        // public GameObject promptTextObject;
        // private Text promptText;
        public ColorOption selectedColorOption;

        private Renderer objectRenderer;
        private Material originalMaterial;
        private Material glowMaterial; 

        public float glowIntensity = 1f;

        private bool isGlowing = false;
        private bool isHacked = false;
        private bool isConected = false;
        private bool isActiv = false;

        public bool testV = true;


        // Start is called before the first frame update
        void Start()
        {
            objectRenderer = controllerPanelObject.GetComponent<Renderer>();
            SetColor(GetColorFromOption(selectedColorOption), false);  
            ishacked = true;
        }

        // Update is called once per frame
        void Update()
        {
            // if (isConected)
            // {
            //     CheckButton();                
            // }        
        }

        private void SetColor(Color newColor, bool isGlowing)
        {
            this.isGlowing = isGlowing;
            if (isGlowing)
            {
                objectRenderer.material.SetColor("_Color", newColor);
                objectRenderer.material.EnableKeyword("_EMISSION");
                objectRenderer.material.SetColor("_EmissionColor", newColor * glowIntensity);
            }
            else
            {
                objectRenderer.material.SetColor("_Color", newColor);
                objectRenderer.material.DisableKeyword("_EMISSION");
              
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isConected = true;
                SetColor(GetColorFromOption(selectedColorOption), isConected);
                // promptText.enabled = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isConected = false;
                SetColor(GetColorFromOption(selectedColorOption), isConected);

            }
        }

        private void OnEnable()
        {
            InputManager.Instance.ApplyOpenEvent += CheckButton;
        }

        private void OnDisable()
        {
            InputManager.Instance.ApplyOpenEvent -= CheckButton;
        }

        private void CheckButton()
        {
            if (isConected)
            {
                if (!isHacked)
                {
                    isActiv = !isActiv;
                    setColorActiv(selectedColorOption, isActiv);
                }
                else
                {
                    Debug.Log("Start hacking");
                    Debug.Log(password);
                    isHacked = false;
                } 
            }
        } 

        private void setColorActiv(ColorOption option, bool isActiv)
        {
            if (option == ColorOption.Белый)
            {
                BuilderManager.Instance.isActivWhite = isActiv;
            }
            else if (option == ColorOption.Красный)
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

        public void set_hacked()
        {            
            ishacked = !ishacked;

           
        }

        public void SetPassword(string value, bool hacked)
        {
            hasPassword = hacked;
            password = value;
            isHacked = hasPassword;

        }

        public void set_color_index(int value)
        {
            color_index = value;
            selectedColorOption = (ColorOption)value;
            SetColor(GetColorFromOption(selectedColorOption), false);
        }

        public override void SetColorIndex(int value)
        {
            color_index = value;
            selectedColorOption = (ColorOption)value;
            SetColor(GetColorFromOption(selectedColorOption), false);
        }

        public override void SetActive(bool active)
        {
            
        }
    }
}
