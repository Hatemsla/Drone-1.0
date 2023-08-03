using System;
using System.Collections.Generic;
using Builder;
using UnityEngine;

namespace Drone
{
    public class Lamp : InteractiveObject
    {
        [SerializeField] private Light lamp;
        [SerializeField] private MeshRenderer[] meshes;
        public GameObject lampObject;

        private Renderer objectRenderer;


        public enum ColorOption
        {
            Белый,
            Красный,
            Зелёный,
            Жёлтый,
            Синий
        }
        public ColorOption selectedColorOption;

        private Dictionary<Material, bool> _emissions = new Dictionary<Material, bool>();

        private void Start()
        {
            //objectRenderer = lampObject.GetComponent<Renderer>();
            SetColor(GetColorFromOption(selectedColorOption));
            
            //TurnOff();
        }

        public void TurnOn()
        {
            isLampTurn = true;
            lamp.enabled = isLampTurn;
            
            foreach (var mesh in meshes)
            {
                foreach (var mat in mesh.materials)
                {
                    if (_emissions.TryGetValue(mat, out var isEmissionEnabled) && isEmissionEnabled)
                    {
                        Debug.Log(selectedColorOption);
                        mat.SetColor("_EmissionColor", GetColorFromOption(selectedColorOption));
                        mat.EnableKeyword("_EMISSION");
                    }
                }
            }
        }
        
        public void TurnOff()
        {
            isLampTurn = false;
            lamp.enabled = isLampTurn;
            
            foreach (var mesh in meshes)
            {
                foreach (var mat in mesh.materials)
                {
                    if (_emissions.TryGetValue(mat, out var isEmissionEnabled) && isEmissionEnabled)
                    {
                        mat.DisableKeyword("_EMISSION");
                    }
                }
            }
        }

        public void TurnLamp()
        {
            isLampTurn = !isLampTurn;
            lamp.enabled = isLampTurn;

            foreach (var mesh in meshes)
            {
                foreach (var mat in mesh.materials)
                {
                    if (isLampTurn && _emissions.TryGetValue(mat, out var isEmissionEnabled) && isEmissionEnabled)
                    {
                        mat.SetColor("_EmissionColor", GetColorFromOption(selectedColorOption));
                        mat.EnableKeyword("_EMISSION");
                    }
                    else
                    {
                        mat.DisableKeyword("_EMISSION");
                    }
                        

                }
            }
        }

        private void SetColor(Color newColor)
        {
            lamp.color = GetColorFromOption(selectedColorOption);
            //objectRenderer.material.color = newColor;
            //objectRenderer.material.SetColor("_EmissionColor", GetColorFromOption(selectedColorOption));
            foreach (var mesh in meshes)
            {
                foreach (var mat in mesh.materials)
                {
                    if (isLampTurn && _emissions.TryGetValue(mat, out var isEmissionEnabled) && isEmissionEnabled)
                    {
                        mat.SetColor("_EmissionColor", GetColorFromOption(selectedColorOption));
                        mat.SetColor("_Color", GetColorFromOption(selectedColorOption));

                    }
                }
            }            
        }

        private Color GetColorFromOption(ColorOption option)
        {
            switch (option)
            {
                case ColorOption.Белый:
                    return Color.white;
                case ColorOption.Красный:
                    return Color.red;
                case ColorOption.Зелёный:
                    return Color.green;
                case ColorOption.Жёлтый:
                    return Color.yellow;
                case ColorOption.Синий:
                    return Color.blue;
                default:
                    return Color.white;
            }
        }

        public override void SetActive(bool active)
        {
            isActive = active;
            if(isActive)
                TurnOn();
            else
                TurnOff();

        }

        public override void SetColorIndex(int value)
        {
            color_index = value;
            selectedColorOption = (ColorOption)value;
            SetColor(GetColorFromOption(selectedColorOption));

        }
    }
}