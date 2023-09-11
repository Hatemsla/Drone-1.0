using System.Collections;
using System.Collections.Generic;
using Drone;
using UnityEngine;

namespace Drone.Builder.ControllerElements
{
    public class ControledGate : InteractiveObject
    {
        public GameObject gateObject;
        // public bool is_activ;
        private MeshRenderer mesh;
        private Collider col;
        private Renderer objectRenderer;

        public float glowIntensity = 1f;

        public ColorOption selectedColorOption;
        // private bool isActiv;
        // private bool previousIsActiv;

        // Start is called before the first frame update
        void Start()
        {
            objectRenderer = gateObject.GetComponent<Renderer>();
            SetColor(GetColorFromOption(selectedColorOption)); 
            mesh = gateObject.GetComponent<MeshRenderer>();
            col = gateObject.GetComponent<Collider>();
            if (!isActive)
            {
                DisableMeshAndCollider();
            }        
        }

        // Update is called once per frame
        void Update()
        {
            if (CheckColorActiveChange(selectedColorOption))
            {
                Debug.Log("CheckColorActivChange");
                if (col.enabled)
                {
                    DisableMeshAndCollider();
                }
                else 
                {
                    EnableMeshAndCollider();
                }
            }        
        }

        private void DisableMeshAndCollider()
        {
            mesh.enabled = false;
            col.enabled = false;
        }

        private void EnableMeshAndCollider()
        {
            mesh.enabled = true;
            col.enabled = true;
        }

        private void SetColor(Color newColor)
        {            
            objectRenderer.material.SetColor("_Color", newColor);
            objectRenderer.material.EnableKeyword("_EMISSION");
            objectRenderer.material.SetColor("_EmissionColor", newColor * glowIntensity);
                        
        }

        // private bool CheckColorActivChange(ColorOption option)
        // {
        //     if (option == ColorOption.Красный)
        //     {
        //         isActiv = BuilderManager.Instance.isActivRed;
        //     }
        //     else if (option == ColorOption.Зелёный)
        //     {
        //         isActiv = BuilderManager.Instance.isActivGreen;
        //     }
        //     else if (option == ColorOption.Жёлтый)
        //     {
        //         isActiv = BuilderManager.Instance.isActivYellow;
        //     }
        //     else if (option == ColorOption.Синий)
        //     {
        //         isActiv = BuilderManager.Instance.isActivBlue;
        //     }
        //     if (previousIsActiv != isActiv)
        //     {
        //         previousIsActiv = isActiv;
        //         return true;
        //     }
        //     else
        //     {
        //         return false;
        //     }
        // }

        public void set_color_index(int value)
        {
            colorIndex = value;
            selectedColorOption = (ColorOption)value;
            SetColor(GetColorFromOption(selectedColorOption));
        }

        public override void SetColorIndex(int value)
        {
            colorIndex = value;
            selectedColorOption = (ColorOption)value;
            SetColor(GetColorFromOption(selectedColorOption));
        }

        public override void SetActive(bool active)
        {
            isActive = active;
            if (!isActive)
            {
                DisableMeshAndCollider();
            }
            else 
            {
                EnableMeshAndCollider();
            }
        }
    }
}
