using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DroneFootball
{
    [RequireComponent(typeof(CheckNode), typeof(Rigidbody))]
    public class DroneFootballAI : MonoBehaviour
    {
        public float proportionalGain;
        public float integralGain;
        public float derivativeGain;
        public float outputMin = -1;
        public float outputMax = 1;
        public float integralSaturation;

        public float minMaxPitch;
        public float minMaxRoll;
        public float yawPower;
        public Vector2 cyclic;
        public float pedals;
        public float throttle;
        public float lerpSpeed;
        public Rigidbody rb;
        public Vector3 directionToPlayer;
        public float upPointDistance;
        public Transform playerTransform;
        public CheckNode checkNode;
        public FootballController footballController;
        private List<DroneEngine> _engines;

        private float _finalPitch;
        private float _finalRoll;
        private float _finalYaw;
        private float _isMove;
        private PIDController _pitchController;
        private PIDController _rollController;

        private PIDController _throttleController;
        private float _yaw;
        private PIDController _yawController;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            _engines = GetComponentsInChildren<DroneEngine>().ToList();
            checkNode = GetComponent<CheckNode>();

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
            foreach (var engine in _engines) engine.UpdateEngine(rb, throttle);
            throttle = _throttleController.UpdateThrottle(Time.fixedDeltaTime, rb.position.y,
                playerTransform.position.y);

            _finalPitch =
                _pitchController.UpdateThrottle(Time.fixedDeltaTime, rb.position.x, playerTransform.position.x);
            _finalRoll = _rollController.UpdateThrottle(Time.fixedDeltaTime, rb.position.z, playerTransform.position.z);

            var targetPosition = playerTransform.position;
            targetPosition.y = rb.position.y; //ignore difference in Y
            var targetDir = (targetPosition - rb.position).normalized;
            var forwardDir = rb.rotation * Vector3.forward;

            var currentAngle = Vector3.SignedAngle(Vector3.forward, forwardDir, Vector3.up);
            var targetAngle = Vector3.SignedAngle(Vector3.forward, targetDir, Vector3.up);

            _finalYaw = _yawController.UpdateAngle(Time.fixedDeltaTime, currentAngle, targetAngle);
            rb.AddForce(new Vector3(_finalPitch, 0, _finalRoll) * 3);
            // transform.LookAt(playerTransform);
        }
    }
}