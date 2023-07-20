using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Drone;
using UnityEngine;

namespace Builder
{
    public class DroneBuilderController : DroneController
    {
        public int boostsCount;
        public float jerkForce;
        [SerializeField] private float jerkDelay = 5f;
        [SerializeField] private float shieldDelay = 10f;
        [SerializeField] private GameObject shield;
        public Light flashLight;
        public DroneBuilderCheckNode droneBuilderCheckNode;
        public DroneBuilderSoundController droneBuilderSoundController;
        public DroneRpgController droneRpgController;
        public Rigidbody rb;
        
        private List<DroneEngine> _engines;
        private float _finalPitch;
        private float _finalRoll;
        private float _finalYaw;
        private float _isMove;
        private float _jerkTime;
        private bool _hasShield = true;

        private void Awake()
        {
            droneBuilderCheckNode = GetComponent<DroneBuilderCheckNode>();
            droneBuilderSoundController = GetComponent<DroneBuilderSoundController>();
            droneRpgController = GetComponent<DroneRpgController>();
            BuilderManager.Instance.droneBuilderController = this;
            BuilderManager.Instance.droneBuilderCheckNode = droneBuilderCheckNode;
            BuilderManager.Instance.droneBuilderSoundController = droneBuilderSoundController;
            BuilderManager.Instance.cameraController = GetComponent<BuilderCameraController>();
            rb = GetComponent<Rigidbody>();
            _engines = GetComponentsInChildren<DroneEngine>().ToList();
        }

        private void Start()
        {
            yawPower = BuilderManager.Instance.currentYawSensitivity;
        }

        private void OnEnable()
        {
            InputManager.Instance.FlashlightEvent += TurnFlashLight;
            InputManager.Instance.CyclicEvent += OnCyclic;
            InputManager.Instance.ThrottleEvent += OnThrottle;
            InputManager.Instance.PedalsEvent += OnPedals;
            InputManager.Instance.JerkEvent += OnJerk;
            InputManager.Instance.ShieldEvent += OnShield;
        }

        private void OnThrottle(float value) => throttle = value;
        private void OnPedals(float value) => pedals = value;
        private void OnCyclic(Vector2 value) => cyclic = value;

        private void OnDisable()
        {
            InputManager.Instance.FlashlightEvent -= TurnFlashLight;
            InputManager.Instance.CyclicEvent -= OnCyclic;
            InputManager.Instance.ThrottleEvent -= OnThrottle;
            InputManager.Instance.PedalsEvent -= OnPedals;
            InputManager.Instance.JerkEvent -= OnJerk;
            InputManager.Instance.ShieldEvent -= OnShield;
        }

        private void FixedUpdate()
        {
            if (_jerkTime <= jerkDelay)
                _jerkTime += Time.deltaTime;
            
            currentSpeed = rb.velocity.magnitude / MaxVelocity * 40f;
            currentPercentSpeed = rb.velocity.magnitude / MaxVelocity * 100f;
            if (BuilderManager.Instance.isMove && droneRpgController.isAlive && droneRpgController.isCharged)
            {
                _isMove = 0;
                _isMove = Mathf.Abs(cyclic.x) + Mathf.Abs(cyclic.y) + Mathf.Abs(pedals) + Mathf.Abs(throttle);
                DroneMove();
            }
        }
        
        private void OnShield()
        {
            if (!isShieldActive && _hasShield)
            {
                isShieldActive = true;
                _hasShield = false;
                droneRpgController.ApplyEnergyUsage(5);
                shield.SetActive(true);
                
                StartCoroutine(DisableShield());
            }
        }

        private IEnumerator DisableShield()
        {
            yield return new WaitForSeconds(shieldDelay);
            shield.SetActive(false);
            isShieldActive = false;
        }

        private void OnJerk()
        {
            if (_jerkTime >= 5)
            {
                rb.AddForce(rb.velocity.normalized * jerkForce, ForceMode.Impulse);
                _jerkTime = 0;
            }
        }

        private void TurnFlashLight()
        {
            if (BuilderManager.Instance.isMove)
            {
                flashLight.enabled = !flashLight.enabled;
            }
        }

        private void OnUVFlashLight()
        {
            // if (BuilderManager.Instance.isMove)
            //     uvFlashLight.enabled = !uvFlashLight.enabled;
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

        public IEnumerator IsFreezing()
        {
            BuilderManager.Instance.isMove = false;
            yield return new WaitForSeconds(3f);
            BuilderManager.Instance.isMove = true;
        }
    }
}