using System;
using System.Collections;
using Drone;
using UnityEngine;

namespace Builder
{
    public sealed class PitStop : InteractiveObject
    {
        [SerializeField] private Transform pitStopCenter;
        [SerializeField] private int pitStopCost = 10;
        [SerializeField] private Prompt prompt;
        
        private bool _inTrigger;
        private float _healingTime;
        private float _currentHealingTime;
        private DroneRpgController _drone;
        private HealingStation _healingStation = new();

        private void Start()
        {
            _currentHealingTime = _healingTime + 1;
        }

        private void OnEnable()
        {
            BuilderManager.Instance.TestLevelEvent += FindPrompt;
            InputManager.Instance.ApplyOpenEvent += OpenPitStop;
        }

        private void OnDisable()
        {
            BuilderManager.Instance.TestLevelEvent -= FindPrompt;
            InputManager.Instance.ApplyOpenEvent -= OpenPitStop;
        }

        private void OpenPitStop()
        {
            if (!_inTrigger || !isActive)
                return;
            
            prompt.SetActive(false);
            _healingTime = _healingStation.GetHealingTime(_drone.Health + _drone.Armor);
            _currentHealingTime = 0f;
            
            StartCoroutine(RestoreDrone(_healingTime));
        }

        private IEnumerator RestoreDrone(float healingTime)
        {
            var elapsedTime = 0f;
            var startHealth = _drone.Health;
            var startArmor = _drone.Armor;
            var targetHealth = 100f;
            var targetArmor = 100f;

            InputManager.Instance.TurnCustomActionMap("");
            _drone.transform.position = pitStopCenter.position;
            BuilderManager.Instance.builderUI.restoreHealthText.enabled = true;

            while (elapsedTime < healingTime && _inTrigger)
            {
                elapsedTime += Time.deltaTime;
                var waitTime = healingTime - elapsedTime;
                BuilderManager.Instance.builderUI.restoreHealthText.text = $"Время починки: {waitTime:f1}";
                var progress = elapsedTime / healingTime;
                var currentHealth = Mathf.Lerp(startHealth, targetHealth, progress);
                var currentArmor = Mathf.Lerp(startArmor, targetArmor, progress);

                _drone.Health = currentHealth;
                _drone.Armor = currentArmor;

                yield return null;
            }
            
            InputManager.Instance.TurnCustomActionMap(Idents.ActionMaps.Player);
            BuilderManager.Instance.builderUI.restoreHealthText.enabled = false;
        }

        private void FindPrompt()
        {
            prompt = BuilderManager.Instance.builderUI.prompt;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!BuilderManager.Instance.isMove)
                return;
            
            _inTrigger = true;
            prompt.PromptText = Idents.Tags.PromptText.DefaultText;
            _drone = other.GetComponentInParent<DroneRpgController>();
            prompt.SetActive(true);
        }

        private void OnTriggerExit(Collider other)
        {
            if(!BuilderManager.Instance.isMove)
                return;
            
            _inTrigger = false;
            prompt.SetActive(false);
            InputManager.Instance.TurnCustomActionMap(Idents.ActionMaps.Player);
            _currentHealingTime = _healingTime;
        }

        public override void SetActive(bool active)
        {
            isActive = active;
        }

        public override void SetColorIndex(int value)
        {
        }
    }
}