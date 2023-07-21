using System;
using Builder;
using UnityEngine;

namespace Drone
{
    public class Pendulum : InteractiveObject
    {
        private Rigidbody _rb;
        private bool _movingClockwise;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _movingClockwise = true;
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
        }
    }
}