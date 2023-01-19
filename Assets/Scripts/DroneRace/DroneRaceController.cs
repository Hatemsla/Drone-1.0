using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DroneRace
{
    [RequireComponent(typeof(Rigidbody), typeof(DroneRaceCheckNode))]
    public class DroneRaceController : MonoBehaviour
    {
        public float minMaxPitch;
        public float minMaxRoll;
        public float yawPower;
        public Vector2 cyclic;
        public float pedals;
        public float throttle;
        public float lerpSpeed;
        public bool isSimpleMode;
        public DroneRaceCheckNode droneRaceCheckNode;
        public RaceController raceController;
        public Path pathAI;
        private List<DroneEngine> _engines;
        private float _finalPitch;
        private float _finalRoll;
        private float _finalYaw;
        private float _isMove;

        private Rigidbody _rb;
        private float _yaw;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _engines = GetComponentsInChildren<DroneEngine>().ToList();
            droneRaceCheckNode = GetComponent<DroneRaceCheckNode>();
            droneRaceCheckNode.nodes = pathAI.nodes;
        }

        private void Start()
        {
            droneRaceCheckNode.nodes = pathAI.nodes;
        }

        private void FixedUpdate()
        {
            if (raceController.isGameStart)
            {
                GetInput();
                DroneMove();
            }
        }

        private void GetInput()
        {
            _isMove = 0;
            cyclic.x = Input.GetAxis("Horizontal");
            cyclic.y = Input.GetAxis("Vertical");
            pedals = Input.GetAxis("Pedal");
            throttle = Input.GetAxis("Throttle");
            _isMove = Mathf.Abs(cyclic.x) + Mathf.Abs(cyclic.y) + Mathf.Abs(pedals) + Mathf.Abs(throttle);
        }

        private void DroneMove()
        {
            if (_isMove != 0 || isSimpleMode)
                foreach (var engine in _engines)
                    engine.UpdateEngine(_rb, throttle);

            var pitch = cyclic.y * minMaxPitch;
            var roll = -cyclic.x * minMaxRoll;
            _yaw += pedals * yawPower;

            _finalPitch = Mathf.Lerp(_finalPitch, pitch, Time.deltaTime * lerpSpeed);
            _finalRoll = Mathf.Lerp(_finalRoll, roll, Time.deltaTime * lerpSpeed);
            _finalYaw = Mathf.Lerp(_finalYaw, _yaw, Time.deltaTime * lerpSpeed);

            var rot = Quaternion.Euler(_finalPitch, _finalYaw, _finalRoll);
            _rb.MoveRotation(rot);
        }
    }
}