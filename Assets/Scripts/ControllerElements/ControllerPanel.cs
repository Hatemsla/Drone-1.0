using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Builder
{
    public class ControllerPanel : InteractiveObject
    {

        public enum ColorOption
        {
            Красный, 
            Зелёный,
            Жёлтый,
            Синий            
        }
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
        }

        // Update is called once per frame
        void Update()
        {
            if (isConected)
            {
                CheckButton();                
            }        
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

        private Color GetColorFromOption(ColorOption option)
        {
            switch (option)
            {
                case ColorOption.Красный:
                    return Color.red;
                case ColorOption.Зелёный:
                    return Color.green;
                case ColorOption.Жёлтый:
                    return Color.yellow;
                case ColorOption.Синий:
                    return Color.blue;
                default:
                    return Color.red;
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

        private void CheckButton()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (isHacked)
                {
                    isActiv = !isActiv;
                    setColorActiv(selectedColorOption, isActiv);
                }
                else
                {
                    Debug.Log("Start hacking");
                    isHacked = false;
                }
 
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

        public void set_hacked()
        {
           is_hacked = !is_hacked;
           Debug.Log(is_hacked);
        }

        public void set_conde_n1(int value)
        {
            n1 = value;
            Debug.Log(n1);        
        }

        public void set_conde_n2(int value)
        {
            n2 = value;
            Debug.Log(n2);        
        }

        public void set_conde_n3(int value)
        {
            n3 = value;
            Debug.Log(n3);        
        }

        public void set_color_index(int value)
        {
            Debug.Log(value);
        }
    }
}
