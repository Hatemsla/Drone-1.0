using System.Collections.Generic;
using System.Linq;
using DroneFootball;
using UnityEngine;

namespace DroneRace
{
    [RequireComponent(typeof(Rigidbody), typeof(DroneRaceCheckNode))]
    public class DroneRaceAI : MonoBehaviour
    {
        public float proportionalGain;
        public float integralGain;
        public float derivativeGain;
        public float outputMin = -1;
        public float outputMax = 1;
        public float integralSaturation;
        public float throttle;
        public DroneRaceCheckNode droneRaceCheckNode;
        public DroneRaceController droneRaceController;
        public Path pathAI;
        private List<DroneEngine> _engines;
        private float _finalPitch;
        private float _finalRoll;
        private float _finalYaw;
        private float _isMove;
        private PIDController _pitchController;

        private Rigidbody _rb;
        private PIDController _rollController;
        private PIDController _throttleController;
        private float _yaw;
        private PIDController _yawController;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _engines = GetComponentsInChildren<DroneEngine>().ToList();
            droneRaceCheckNode = GetComponent<DroneRaceCheckNode>();
            droneRaceCheckNode.nodes = pathAI.nodes;

            _throttleController = new PIDController(proportionalGain, integralGain, derivativeGain, outputMin,
                outputMax,
                integralSaturation);
            _pitchController = new PIDController(proportionalGain, integralGain, derivativeGain, outputMin, outputMax,
                integralSaturation);
            _rollController = new PIDController(proportionalGain, integralGain, derivativeGain, outputMin, outputMax,
                integralSaturation);
            _yawController = new PIDController(proportionalGain, integralGain, derivativeGain, outputMin, outputMax,
                integralSaturation);
        }

        private void Start()
        {
            droneRaceCheckNode.nodes = pathAI.nodes;
        }

        private void FixedUpdate()
        {
            if (droneRaceController.isGameStart) DroneMove();
        }

        private void DroneMove()
        {
            var targetPosition = droneRaceCheckNode.nodes[droneRaceCheckNode.currentNode].position;
            foreach (var engine in _engines) engine.UpdateEngine(_rb, throttle);
            throttle = _throttleController.UpdateThrottle(Time.fixedDeltaTime, _rb.position.y, targetPosition.y);

            _finalPitch = _pitchController.UpdateThrottle(Time.fixedDeltaTime, _rb.position.x, targetPosition.x);
            _finalRoll = _rollController.UpdateThrottle(Time.fixedDeltaTime, _rb.position.z, targetPosition.z);

            targetPosition.y = _rb.position.y; //ignore difference in Y
            var targetDir = (targetPosition - _rb.position).normalized;
            var forwardDir = _rb.rotation * Vector3.forward;

            var currentAngle = Vector3.SignedAngle(Vector3.forward, forwardDir, Vector3.up);
            var targetAngle = Vector3.SignedAngle(Vector3.forward, targetDir, Vector3.up);

            _finalYaw = _yawController.UpdateAngle(Time.fixedDeltaTime, currentAngle, targetAngle);
            _rb.AddForce(new Vector3(_finalPitch, 0, _finalRoll));
        }
    }
}