using System.Collections.Generic;
using System.Linq;
using DroneRace;
using UnityEngine;

namespace DroneFootball
{
    [RequireComponent(typeof(DroneFootballCheckNode), typeof(Rigidbody))]
    public class DroneFootballAI : MonoBehaviour
    {
        public float speed;
        public float proportionalGain;
        public float integralGain;
        public float derivativeGain;
        public float outputMin = -1;
        public float outputMax = 1;
        public float integralSaturation;
        public float throttle;
        public Transform targetTransform;
        public DroneFootballCheckNode droneFootballCheckNode;
        public FootballController footballController;
        
        private List<DroneEngine> _engines;
        private Rigidbody _rb;
        private float _finalPitch;
        private float _finalRoll;
        private float _finalYaw;
        private float _isMove;
        private float _yaw;
        private PIDController _pitchController;
        private PIDController _rollController;
        private PIDController _throttleController;
        private PIDController _yawController;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _engines = GetComponentsInChildren<DroneEngine>().ToList();
            droneFootballCheckNode = GetComponent<DroneFootballCheckNode>();

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

        private void FixedUpdate()
        {
            if (footballController.isGameStart) DroneMove();
        }

        private void DroneMove()
        {
            foreach (var engine in _engines) engine.UpdateEngine(_rb, throttle);
            throttle = _throttleController.UpdateThrottle(Time.fixedDeltaTime, _rb.position.y,
                targetTransform.position.y);

            _finalPitch =
                _pitchController.UpdateThrottle(Time.fixedDeltaTime, _rb.position.x, targetTransform.position.x);
            _finalRoll = _rollController.UpdateThrottle(Time.fixedDeltaTime, _rb.position.z, targetTransform.position.z);

            var targetPosition = targetTransform.position;
            targetPosition.y = _rb.position.y; //ignore difference in Y
            var targetDir = (targetPosition - _rb.position).normalized;
            var forwardDir = _rb.rotation * Vector3.forward;

            var currentAngle = Vector3.SignedAngle(Vector3.forward, forwardDir, Vector3.up);
            var targetAngle = Vector3.SignedAngle(Vector3.forward, targetDir, Vector3.up);

            _finalYaw = _yawController.UpdateAngle(Time.fixedDeltaTime, currentAngle, targetAngle);
            _rb.AddForce(new Vector3(_finalPitch, 0, _finalRoll) * speed);
        }
    }
}