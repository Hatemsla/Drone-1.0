using System.Collections.Generic;
using System.Linq;
using Drone.DroneRace;
using UnityEngine;

namespace Drone.DroneFootball
{
    [RequireComponent(typeof(DroneFootballCheckNode), typeof(Rigidbody))]
    public class DroneFootballAI : DroneAI
    {
        public FootballController footballController;
        public MeshRenderer droneMeshRenderer;
        
        private List<DroneEngine> _engines;
        private Rigidbody _rb;
        private Transform _droneTransform;
        private float _finalPitch;
        private float _finalRoll;
        private float _finalYaw;
        private PIDController _pitchController;
        private PIDController _rollController;
        private PIDController _throttleController;

        private void Awake()
        {
            _droneTransform = transform.GetChild(0);
            _rb = GetComponent<Rigidbody>();
            _engines = GetComponentsInChildren<DroneEngine>().ToList();

            _throttleController = new PIDController(proportionalGain, integralGain, derivativeGain, outputMin,
                outputMax,
                integralSaturation);
            _pitchController = new PIDController(proportionalGain, integralGain, derivativeGain, outputMin, outputMax,
                integralSaturation);
            _rollController = new PIDController(proportionalGain, integralGain, derivativeGain, outputMin, outputMax,
                integralSaturation);
        }

        private void FixedUpdate()
        {
            DroneMove();
        }

        private void DroneMove()
        {
            foreach (var engine in _engines) engine.UpdateEngine(_rb, throttle);
            throttle = _throttleController.UpdateThrottle(Time.fixedDeltaTime, _rb.position.y,
                targetTransform.position.y);

            var pitch = _pitchController.UpdateThrottle(Time.fixedDeltaTime, _rb.position.x, targetTransform.position.x);
            var roll = _rollController.UpdateThrottle(Time.fixedDeltaTime, _rb.position.z, targetTransform.position.z);

            var targetPosition = targetTransform.position;
            targetPosition.y = _rb.position.y;

            _rb.AddForce(new Vector3(pitch, 0, roll) * speed);

            _finalPitch = Mathf.Lerp(_finalPitch, pitch * minMaxPitch, Time.deltaTime * lerpSpeed);
            _finalRoll = Mathf.Lerp(_finalRoll, roll * minMaxRoll, Time.deltaTime * lerpSpeed);
            _finalYaw = Vector3.Angle(targetPosition - _droneTransform.localPosition, _droneTransform.forward);
            
            _droneTransform.localEulerAngles = new Vector3(_finalPitch, _finalYaw, _finalRoll);
        }
    }
}