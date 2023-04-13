using System.Collections.Generic;
using System.Linq;
using DroneRace;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Builder
{
    public class DroneBuilderController : MonoBehaviour
    {
        public float minMaxPitch;
        public float minMaxRoll;
        public float yawPower;
        public Vector2 cyclic;
        public float pedals;
        public float throttle;
        public float lerpSpeed;
        public bool isSimpleMode;
        public DroneBuilderCheckNode droneBuilderCheckNode;
        public BuilderManager builderManager;
        public MeshRenderer droneMeshRenderer;
        
        private List<DroneEngine> _engines;
        private float _finalPitch;
        private float _finalRoll;
        private float _finalYaw;
        private float _isMove;
        private Rigidbody _rb;
        private float _yaw;

        private void Awake()
        {
            droneBuilderCheckNode = GetComponent<DroneBuilderCheckNode>();
            builderManager = FindObjectOfType<BuilderManager>();
            builderManager.droneBuilderController = this;
            builderManager.droneBuilderCheckNode = droneBuilderCheckNode;
            _rb = GetComponent<Rigidbody>();
            _engines = GetComponentsInChildren<DroneEngine>().ToList();
        }

        private void FixedUpdate()
        {
            if (builderManager.isMove)
            {
                _isMove = 0;
                _isMove = Mathf.Abs(cyclic.x) + Mathf.Abs(cyclic.y) + Mathf.Abs(pedals) + Mathf.Abs(throttle);
                DroneMove();
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

            var pitch = cyclic.y * minMaxPitch;
            var roll = -cyclic.x * minMaxRoll;
            _yaw += pedals * yawPower;

            _finalPitch = Mathf.Lerp(_finalPitch, pitch, Time.deltaTime * lerpSpeed);
            _finalRoll = Mathf.Lerp(_finalRoll, roll, Time.deltaTime * lerpSpeed);
            _finalYaw = Mathf.Lerp(_finalYaw, _yaw, Time.deltaTime * lerpSpeed);

            var rot = Quaternion.Euler(_finalPitch, _finalYaw, _finalRoll);
            _rb.MoveRotation(rot);
        }

        private void CheckDroneHover()
        {
            if (isSimpleMode && _isMove == 0)
            {
                _rb.drag = 3;
            }
            else
            {
                _rb.drag = 0.5f;
            }
        }
    }
}