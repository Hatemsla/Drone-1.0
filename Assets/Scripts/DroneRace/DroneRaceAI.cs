using System.Collections.Generic;
using System.Linq;
using Drone.DroneFootball;
using UnityEngine;

namespace Drone.DroneRace
{
    [RequireComponent(typeof(Rigidbody), typeof(DroneRaceCheckNode))]
    public class DroneRaceAI : DroneAI
    {
        public DroneRaceCheckNode droneRaceCheckNode;
        public RaceController raceController;
        public MeshRenderer droneMeshRenderer;
        
        private List<DroneEngine> _engines;
        private Rigidbody _rb;
        private Transform _droneTransform;
        private float _finalPitch;
        private float _finalRoll;
        private float _finalYaw;
        private float _isMove;
        private PIDController _pitchController;
        private PIDController _rollController;
        private PIDController _throttleController;
        private PIDController _yawController;

        private void Awake()
        {
            _droneTransform = transform.GetChild(0);
            _rb = GetComponent<Rigidbody>();
            _engines = GetComponentsInChildren<DroneEngine>().ToList();
            droneRaceCheckNode = GetComponent<DroneRaceCheckNode>();

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
            if (raceController.isGameStart) DroneMove();
        }

        private void DroneMove()
        {
            targetTransform = droneRaceCheckNode.nodes[droneRaceCheckNode.currentNode];
            var targetPosition = targetTransform.position;
            foreach (var engine in _engines) engine.UpdateEngine(_rb, throttle);
            throttle = _throttleController.UpdateThrottle(Time.fixedDeltaTime, _rb.position.y, targetPosition.y);

            var pitch = _pitchController.UpdateThrottle(Time.fixedDeltaTime, _rb.position.x, targetPosition.x);
            var roll = _rollController.UpdateThrottle(Time.fixedDeltaTime, _rb.position.z, targetPosition.z);

            targetPosition.y = _rb.position.y;
            
            _rb.AddForce(new Vector3(pitch, 0, roll) * speed);
            
            _finalPitch = Mathf.Lerp(_finalPitch, pitch * minMaxPitch, Time.deltaTime * lerpSpeed);
            _finalRoll = Mathf.Lerp(_finalRoll, roll * minMaxRoll, Time.deltaTime * lerpSpeed);
            _finalYaw = Vector3.Angle(targetPosition - _droneTransform.localPosition, _droneTransform.forward);
            
            _droneTransform.localEulerAngles = new Vector3(_finalPitch, _finalYaw, _finalRoll);
        }
    }
}