using System;
using Drone;
using UnityEngine;

namespace Drone.Builder
{
    public class WindZoneScript : InteractiveObject
    {
        public float windForce;
        public GameObject colorObject;
        private Renderer objectRenderer;
        public ColorOption selectedColorOption;


        private void SetColor(Color scolor)
        {   
            Color newColor = new Color(scolor.r, scolor.g, scolor.b, 0.07843138f);
            objectRenderer.material.SetColor("_Color", newColor);
        }

        private void Start()
        {
            windForce = 0f;
            objectRenderer = colorObject.GetComponent<Renderer>();
            SetColor(GetColorFromOption(selectedColorOption)); 
        }
        private void OnTriggerStay(Collider other)
        {
            if(!isActive)
                return;
            
            var player = other.GetComponent<DroneController>();
            if (player)
            {
                player.GetComponent<Rigidbody>().AddForce(transform.forward * windForce);
            }
        }

        public override void SetActive(bool active)
        {
            isActive = active;
            SetColor(GetColorFromOption((ColorOption)colorIndex));
        }

        public override void SetColorIndex(int value)
        {
            colorIndex = value;
            SetColor(GetColorFromOption((ColorOption)value));
        }
    }
}