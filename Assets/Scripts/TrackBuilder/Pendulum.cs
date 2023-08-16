using System;
using Drone.Builder;
using UnityEngine;

namespace Drone.Builder
{
    public class Pendulum : InteractiveObject
    {
        private Rigidbody _rb;
        private bool _movingClockwise;
        public GameObject colorObject;
        private Renderer objectRenderer;
        public float glowIntensity = 1f;
        public ColorOption selectedColorOption;


        private void SetColor(Color newColor)
        {
            if (isActive)
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

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _movingClockwise = true;

            leftPendulumAngle = 0.6f;
            rightPendulumAngle = 0.6f;
            pendulumMoveSpeed = 5;
            objectRenderer = colorObject.GetComponent<Renderer>();
            SetColor(GetColorFromOption(selectedColorOption)); 
        }

        public void Update()
        {
            if (isActive) Move();
        }

        private void ChangeMoveDir()
        {
            if (transform.rotation.z > rightPendulumAngle)
            {
                _movingClockwise = false;
            }
            if (transform.rotation.z < leftPendulumAngle)
            {
                _movingClockwise = true;
            }

        }

        private void Move()
        {
            ChangeMoveDir();

            switch (_movingClockwise)
            {
                case true:
                    _rb.angularVelocity = new Vector3(0f, 0f, pendulumMoveSpeed) * pendulumMoveSpeed;
                    break;
                case false:
                    _rb.angularVelocity = new Vector3(0f, 0f, -pendulumMoveSpeed) * pendulumMoveSpeed;
                    break;
            }
        }

        public override void SetActive(bool active)
        {
            isActive = active;
            SetColor(GetColorFromOption((ColorOption)color_index));
        }

        public override void SetColorIndex(int value)
        {
            color_index = value;
            SetColor(GetColorFromOption((ColorOption)value));
        }
    }
}