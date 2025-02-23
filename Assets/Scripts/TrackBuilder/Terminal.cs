using System;
using Drone.Builder;
using UnityEngine;

namespace Drone
{
    public class Terminal : InteractiveObject
    {
        [SerializeField] private Prompt prompt;
        public DroneRpgController droneRpgController;

        private bool _inTrigger;

        public event Action<Terminal> TerminalOpenEvent;

        private void OnEnable()
        {
            InputManager.Instance.ApplyOpenEvent += OpenTerminal;
            BuilderManager.Instance.ObjectChangeSceneEvent += FindPrompt;
        }

        private void OnDisable()
        {
            InputManager.Instance.ApplyOpenEvent -= OpenTerminal;
            BuilderManager.Instance.ObjectChangeSceneEvent -= FindPrompt;
        }

        private void FindPrompt()
        {
            prompt = BuilderManager.Instance.builderUI.prompt;
        }

        private void OpenTerminal()
        {
            if (!_inTrigger || !isActive)
                return;

            TerminalOpenEvent?.Invoke(this);
            droneRpgController.isEnergyUsage = false;
            InputManager.Instance.TurnCustomActionMap("Terminal");
            BuilderManager.Instance.builderUI.droneView.SetActive(false);
            BuilderManager.Instance.builderUI.objectEditPanel.SetActive(false);
            BuilderManager.Instance.builderUI.editButtons.SetActive(false);
            BuilderManager.Instance.builderUI.terminalUI.SetActive(true);
            prompt.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!BuilderManager.Instance.isMove || !isActive)
                return;
            
            if (other.GetComponentInParent<DroneController>() is DroneBuilderController drone)
            {
                droneRpgController = drone.droneRpgController;
                _inTrigger = true;
                prompt.PromptText = Idents.Tags.PromptText.DefaultText;
                prompt.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if(!BuilderManager.Instance.isMove || !isActive)
                return;

            if (other.GetComponentInParent<DroneController>() is DroneBuilderController drone)
            {
                CloseTerminal();
                drone.droneRpgController.isEnergyUsage = true;
                _inTrigger = false;
                prompt.SetActive(false);
            }
        }
        
        private void CloseTerminal()
        {
            InputManager.Instance.TurnCustomActionMap("Player");
            BuilderManager.Instance.builderUI.droneView.SetActive(true);
            BuilderManager.Instance.builderUI.terminalUI.SetActive(false);
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