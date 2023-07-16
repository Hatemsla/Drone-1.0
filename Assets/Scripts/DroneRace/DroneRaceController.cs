using System.Collections.Generic;
using System.Linq;
using Drone;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DroneRace
{
    [RequireComponent(typeof(Rigidbody), typeof(DroneRaceCheckNode))]
    public class DroneRaceController : DroneController
    {
        public RaceController raceController;
        [SerializeField] private Rigidbody rb;
        
        private List<DroneEngine> _engines;
        private float _finalPitch;
        private float _finalRoll;
        private float _finalYaw;
        private float _isMove;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            _engines = GetComponentsInChildren<DroneEngine>().ToList();
        }
        
        private void OnEnable()
        {
            InputManager.Instance.CyclicEvent += OnCyclic;
            InputManager.Instance.ThrottleEvent += OnThrottle;
            InputManager.Instance.PedalsEvent += OnPedals;
        }

        private void OnThrottle(float value) => throttle = value;
        private void OnPedals(float value) => pedals = value;
        private void OnCyclic(Vector2 value) => cyclic = value;

        private void OnDisable()
        {
            InputManager.Instance.CyclicEvent -= OnCyclic;
            InputManager.Instance.ThrottleEvent -= OnThrottle;
            InputManager.Instance.PedalsEvent -= OnPedals;
        }
        
        private void FixedUpdate()
        {
            currentSpeed = rb.velocity.magnitude / MaxVelocity * 40f;
            currentPercentSpeed = rb.velocity.magnitude / MaxVelocity * 100f;
            if (raceController.isGameStart)
            {
                _isMove = 0;
                _isMove = Mathf.Abs(cyclic.x) + Mathf.Abs(cyclic.y) + Mathf.Abs(pedals) + Mathf.Abs(throttle);
                DroneMove();
            }
        }

        private void DroneMove()
        {
            if (_isMove != 0 || isSimpleMode)
            {
                foreach (var engine in _engines)
                {
                    engine.UpdateEngine(rb, throttle);
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
            rb.MoveRotation(rot);
        }

        private void CheckDroneHover()
        {
            if (isSimpleMode && _isMove == 0)
            {
                rb.drag = 5;
            }
            else
            {
                rb.drag = 0.5f;
            }
        }
    }
}