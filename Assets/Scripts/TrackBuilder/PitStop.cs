using System;
using System.Collections;
using Drone;
using UnityEngine;

namespace Drone.Builder
{
    public sealed class PitStop : InteractiveObject
    {
        [SerializeField] private Transform pitStopCenter;
        [SerializeField] private ParticleEffect restoreEffect;
        [SerializeField] private Prompt prompt;
        [SerializeField] private AudioSource restoreSound;
        
        private bool _inTrigger;
        private float _healingTime;
        private DroneRpgController _drone;
        private HealingStation _healingStation = new();

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

            StartCoroutine(RestoreDrone(_healingTime));
        }

        private IEnumerator RestoreDrone(float healingTime)
        {
            restoreSound.Play();
            var elapsedTime = 0f;
            var startHealth = _drone.Health;
            var startArmor = _drone.Armor;
            var targetHealth = 100f;
            var targetArmor = 100f;

            InputManager.Instance.TurnCustomActionMap("");
            StartCoroutine(TrackBuilderUtils.SetPositionSmoothly(_drone.transform, _drone.transform.position,pitStopCenter.position));
            BuilderManager.Instance.builderUI.restoreHealthText.enabled = true;

            while (elapsedTime < healingTime && _inTrigger)
            {
                if(elapsedTime >= 0.5f && !restoreEffect.Effect.isPlaying)
                    restoreEffect.Effect.Play();
                
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
            restoreEffect.Effect.Stop();
            restoreSound.Stop();
        }

        private void FindPrompt()
        {
            prompt = BuilderManager.Instance.builderUI.prompt;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!BuilderManager.Instance.isMove || !isActive)
                return;
            
            _inTrigger = true;
            prompt.PromptText = Idents.Tags.PromptText.DefaultText;
            _drone = other.GetComponentInParent<DroneRpgController>();
            prompt.SetActive(true);
        }

        private void OnTriggerExit(Collider other)
        {
            if(!BuilderManager.Instance.isMove || !isActive)
                return;
            
            _inTrigger = false;
            prompt.SetActive(false);
            InputManager.Instance.TurnCustomActionMap(Idents.ActionMaps.Player);
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