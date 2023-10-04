using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Drone;
using Unity.VisualScripting;
using UnityEngine;

namespace Drone.Builder
{
    public class DroneBuilderController : DroneController
    {
        [SerializeField] private float jerkForce;
        [SerializeField] private float jerkDelay = 5f;
        [SerializeField] private float shieldDelay = 10f;
        [SerializeField] private GameObject shield;
        [SerializeField] private Light flashLight;
        [SerializeField] private DroneBuilderCheckNode droneBuilderCheckNode;
        [SerializeField] private DroneBuilderSoundController droneBuilderSoundController;
        public DroneRpgController droneRpgController;
        public Rigidbody rb;
        
        private List<DroneEngine> _engines;
        private float _finalPitch;
        private float _finalRoll;
        private float _finalYaw;
        private float _isMove;
        private float _jerkTime;
        private bool _isShieldActive = true;

        private void Awake()
        {
            droneBuilderCheckNode = GetComponent<DroneBuilderCheckNode>();
            droneBuilderSoundController = GetComponent<DroneBuilderSoundController>();
            droneRpgController = GetComponent<DroneRpgController>();
            BuilderManager.Instance.droneBuilderController = this;
            BuilderManager.Instance.droneBuilderCheckNode = droneBuilderCheckNode;
            BuilderManager.Instance.droneBuilderSoundController = droneBuilderSoundController;
            BuilderManager.Instance.droneRpgController = droneRpgController;
            rb = GetComponent<Rigidbody>();
            _engines = GetComponentsInChildren<DroneEngine>().ToList();
        }

        private void Start()
        {
            isSimpleMode = BuilderManager.Instance.isSimpleMode;
            yawPower = BuilderManager.Instance.currentYawSensitivity;
            BuilderManager.Instance.StartGame += ActivatePhysics;
            BuilderManager.Instance.StopGame += DeactivatePhysics;
        }

        private void OnEnable()
        {
            InputManager.Instance.FlashlightEvent += TurnFlashLight;
            InputManager.Instance.CyclicEvent += OnCyclic;
            InputManager.Instance.ThrottleEvent += OnThrottle;
            InputManager.Instance.PedalsEvent += OnPedals;
            InputManager.Instance.JerkEvent += OnJerk;
            InputManager.Instance.ShieldEvent += OnShield;
            InputManager.Instance.HpRestoreEvent += OnHpRestore;
            InputManager.Instance.ArmorRestoreEvent += OnArmorRestore;
        }

        private void OnDisable()
        {
            InputManager.Instance.FlashlightEvent -= TurnFlashLight;
            InputManager.Instance.CyclicEvent -= OnCyclic;
            InputManager.Instance.ThrottleEvent -= OnThrottle;
            InputManager.Instance.PedalsEvent -= OnPedals;
            InputManager.Instance.JerkEvent -= OnJerk;
            InputManager.Instance.ShieldEvent -= OnShield;
            InputManager.Instance.HpRestoreEvent -= OnHpRestore;
            InputManager.Instance.ArmorRestoreEvent -= OnArmorRestore;
            BuilderManager.Instance.StartGame -= ActivatePhysics;
            BuilderManager.Instance.StopGame -= DeactivatePhysics;
        }

        private void ActivatePhysics()
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        
        private void DeactivatePhysics()
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        
        private void OnThrottle(float value) => throttle = value;
        private void OnPedals(float value) => pedals = value;
        private void OnCyclic(Vector2 value) => cyclic = value;

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
        
        private void OnHpRestore()
        {
            if (droneRpgController.SkillsCount[Skills.HpRestore] > 0)
            {
                droneRpgController.UpdateSkillValue(Skills.HpRestore, droneRpgController.SkillsCount[Skills.HpRestore] - 1);
                droneRpgController.Health += 15f;
            }
        }

        private void OnArmorRestore()
        {
            if (droneRpgController.SkillsCount[Skills.ArmorRestore] > 0)
            {
                droneRpgController.UpdateSkillValue(Skills.ArmorRestore, droneRpgController.SkillsCount[Skills.ArmorRestore] - 1);
                droneRpgController.Armor += 20f;
            }
        }

        private void OnShield()
        {
            if (!isShieldActive && _isShieldActive && droneRpgController.SkillsCount[Skills.Shield] > 0)
            {
                droneRpgController.UpdateSkillValue(Skills.Shield, droneRpgController.SkillsCount[Skills.Shield] - 1);
                isShieldActive = true;
                _isShieldActive = false;
                droneRpgController.ApplyEnergyUsage(5);
                shield.SetActive(true);
                droneBuilderSoundController.activateShieldSound.Play();
                
                StartCoroutine(DisableShield());
            }
        }

        private IEnumerator DisableShield()
        {
            yield return new WaitForSeconds(shieldDelay);
            droneBuilderSoundController.deactivateShieldSound.Play();
            shield.SetActive(false);
            isShieldActive = false;
        }

        private void OnJerk()
        {
            if (_jerkTime >= jerkDelay && droneRpgController.SkillsCount[Skills.Jerk] > 0)
            {
                droneRpgController.UpdateSkillValue(Skills.Jerk, droneRpgController.SkillsCount[Skills.Jerk] - 1);
                rb.AddForce(rb.velocity.normalized * jerkForce, ForceMode.Impulse);
                _jerkTime = 0;
            }
        }

        private void TurnFlashLight()
        {
            if (BuilderManager.Instance.isMove && droneRpgController.SkillsCount[Skills.Flashlight] > 0)
            {
                flashLight.enabled = !flashLight.enabled;
                droneBuilderSoundController.flashlightSound.Play();
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
                foreach (var engine in _engines)
                    engine.UpdateEngine(rb, throttle * targetSpeed);
            else if (_isMove == 0 && !isSimpleMode)
                if (!isFastDownMode)
                    foreach (var engine in _engines)
                        engine.UpdateEngine(rb, slowDownSpeed * targetSpeed);

            CheckDroneHover();

            var pitch = cyclic.y * minMaxPitch * targetSpeed;
            var roll = -cyclic.x * minMaxRoll * targetSpeed;
            yaw += pedals * yawPower * targetSpeed;

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