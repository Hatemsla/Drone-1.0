using System;
using Builder;
using UnityEngine;

namespace Drone
{
    public class Pendulum : MonoBehaviour
    {
        private Rigidbody _rb;

        public float moveSpeed;
        public float leftAngle;
        public float rightAngle;

        private bool _movingClockwise;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _movingClockwise = true;
        }

        public void Update()
        {
            Move();
        }

        private void ChangeMoveDir()
        {
            if (transform.rotation.z > rightAngle)
            {
                _movingClockwise = false;
            }
            if (transform.rotation.z < leftAngle)
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
                    _rb.angularVelocity = new Vector3(0f, 0f, moveSpeed) * moveSpeed;
                    break;
                case false:
                    _rb.angularVelocity = new Vector3(0f, 0f, -moveSpeed) * moveSpeed;
                    break;
            }
        }
    }
}