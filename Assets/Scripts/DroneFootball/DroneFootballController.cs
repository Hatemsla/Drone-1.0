using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Sockets;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DroneFootball
{
    public class DroneFootballController : DroneController
    {
        public DroneFootballCheckNode droneFootballCheckNode;
        public FootballController footballController;
        public MeshRenderer droneMeshRenderer;

        private List<DroneEngine> _engines;
        private Rigidbody _rb;
        private float _finalPitch;
        private float _finalRoll;
        private float _finalYaw;
        private float _isMove;
        private bool _isStopAlready;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _engines = GetComponentsInChildren<DroneEngine>().ToList();
            droneFootballCheckNode = GetComponent<DroneFootballCheckNode>();
            footballController = FindObjectOfType<FootballController>();
            footballController.droneFootballController = this;
            isSimpleMode = footballController.isSimpleMode;
            footballController.playerCamera = FindObjectOfType<CinemachineBrain>();
        }

        private void Start()
        {
            FindObjectOfType<Server>().droneFootballController = this;
        }

        private void FixedUpdate()
        {
            if (footballController.isGameStart && !isStop)
            {
                _isMove = 0;
                _isMove = Mathf.Abs(cyclic.x) + Mathf.Abs(cyclic.y) + Mathf.Abs(pedals) + Mathf.Abs(throttle);
                DroneMove();
            }
            else if(footballController.isGameStart && !_isStopAlready)
            {
                StartCoroutine(DroneStop());
            }
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
            
            float pitch = cyclic.y * minMaxPitch;
            float roll = -cyclic.x * minMaxRoll;
            yaw += pedals * yawPower;

            _finalPitch = Mathf.Lerp(_finalPitch, pitch, Time.deltaTime * lerpSpeed);
            _finalRoll = Mathf.Lerp(_finalRoll, roll, Time.deltaTime * lerpSpeed);
            _finalYaw = Mathf.Lerp(_finalYaw, yaw, Time.deltaTime * lerpSpeed);
            
            Quaternion rot = Quaternion.Euler(_finalPitch, _finalYaw, _finalRoll);
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

        private IEnumerator DroneStop()
        {
            _isStopAlready = true;
            isStop = true;
            _rb.drag = 10;
            _rb.angularDrag = 10;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            yield return new WaitForSeconds(2);
            _isStopAlready = false;
            isStop = false;
        }
    }
}