using System;
using Drone.Builder;
using UnityEngine;

namespace Drone.Builder
{
    public class Pendulum : InteractiveObject
    {
        public float pendulumMoveSpeed;
        public float leftPendulumAngle;
        public float rightPendulumAngle;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Renderer objectRenderer;
        [SerializeField] private float glowIntensity = 1f;
        [SerializeField] private ColorOption selectedColorOption;
        private bool _movingClockwise;

        private void Start()
        {
            _movingClockwise = true;

            SetColor(GetColorFromOption(selectedColorOption), objectRenderer, glowIntensity); 
        }
        private void Update()
        {
            if (CheckColorActiveChange(selectedColorOption))
            {
                isActive = !isActive;
                SetActive(isActive);
            } 
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
                    rb.angularVelocity = new Vector3(0f, 0f, pendulumMoveSpeed) * pendulumMoveSpeed;
                    break;
                case false:
                    rb.angularVelocity = new Vector3(0f, 0f, -pendulumMoveSpeed) * pendulumMoveSpeed;
                    break;
            }
        }

        public override void SetActive(bool active)
        {
            isActive = active;
            SetColor(GetColorFromOption((ColorOption)colorIndex), objectRenderer, glowIntensity);
        }

        public override void SetColorIndex(int value)
        {
            colorIndex = value;
            SetColor(GetColorFromOption((ColorOption)value), objectRenderer, glowIntensity);
        }
    }
}