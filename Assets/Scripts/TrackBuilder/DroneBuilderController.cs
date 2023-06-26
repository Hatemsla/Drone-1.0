using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Drone;
using DroneFootball;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Builder
{
    public class DroneBuilderController : DroneController
    {
        public int boostsCount;
        public Light flashLight;
        public DroneBuilderCheckNode droneBuilderCheckNode;
        public DroneBuilderSoundController droneBuilderSoundController;
        public DroneRpgController droneRpgController;
        
        private List<DroneEngine> _engines;
        private float _finalPitch;
        private float _finalRoll;
        private float _finalYaw;
        private float _isMove;
        private Rigidbody _rb;
        
        private void Awake()
        {
            droneBuilderCheckNode = GetComponent<DroneBuilderCheckNode>();
            droneBuilderSoundController = GetComponent<DroneBuilderSoundController>();
            droneRpgController = GetComponent<DroneRpgController>();
            BuilderManager.Instance.droneBuilderController = this;
            BuilderManager.Instance.droneBuilderCheckNode = droneBuilderCheckNode;
            BuilderManager.Instance.droneBuilderSoundController = droneBuilderSoundController;
            BuilderManager.Instance.cameraController = GetComponent<BuilderCameraController>();
            _rb = GetComponent<Rigidbody>();
            _engines = GetComponentsInChildren<DroneEngine>().ToList();
        }

        private void Start()
        {
            yawPower = BuilderManager.Instance.currentYawSensitivity;
        }

        private void FixedUpdate()
        {
            currentSpeed = _rb.velocity.magnitude / 8.2f * 40f;
            currentPercentSpeed = _rb.velocity.magnitude / 8.2f * 100f;
            if (BuilderManager.Instance.isMove && droneRpgController.isAlive && droneRpgController.isCharged)
            {
                _isMove = 0;
                _isMove = Mathf.Abs(cyclic.x) + Mathf.Abs(cyclic.y) + Mathf.Abs(pedals) + Mathf.Abs(throttle);
                DroneMove();
            }
        }

        private void OnFlashLight()
        {
            if(BuilderManager.Instance.isMove)
                flashLight.enabled = !flashLight.enabled;
        }

        private void OnCyclic(InputValue value)
        {
            cyclic = value.Get<Vector2>();
        }
        
        private void OnPedals(InputValue value)
        {
            pedals = value.Get<float>();
        }
        
        private void OnThrottle(InputValue value)
        {
            throttle = value.Get<float>();
        }

        private void DroneMove()
        {
            if (_isMove != 0 || isSimpleMode)
            {
                foreach (var engine in _engines)
                {
                    engine.UpdateEngine(_rb, throttle);
                }
            }
            
            CheckDroneHover();

            var pitch = cyclic.y * minMaxPitch;
            var roll = -cyclic.x * minMaxRoll;
            yaw += pedals * yawPower;

            _finalPitch = Mathf.Lerp(_finalPitch, pitch, Time.deltaTime * lerpSpeed);
            _finalRoll = Mathf.Lerp(_finalRoll, roll, Time.deltaTime * lerpSpeed);
            _finalYaw = Mathf.Lerp(_finalYaw, yaw, Time.deltaTime * lerpSpeed);

            var rot = Quaternion.Euler(_finalPitch, _finalYaw, _finalRoll);
            _rb.MoveRotation(rot);
        }

        private void CheckDroneHover()
        {
            if (isSimpleMode && _isMove == 0)
            {
                _rb.drag = 5;
            }
            else
            {
                _rb.drag = 0.5f;
            }
        }

        public IEnumerator IsFreezing()
        {
            BuilderManager.Instance.isMove = false;
            yield return new WaitForSeconds(3f);
            BuilderManager.Instance.isMove = true;
        }
    }
}