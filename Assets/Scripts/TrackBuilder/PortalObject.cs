using System;
using UnityEngine;

namespace Drone.Builder
{
    public sealed class PortalObject : InteractiveObject
    {
        [SerializeField] private Prompt prompt;

        private GameObject _drone;
        
        private void OnEnable()
        {
            InputManager.Instance.ApplyOpenEvent += Teleport;
            BuilderManager.Instance.TestLevelEvent += FindPrompt;
        }

        private void OnDisable()
        {
            InputManager.Instance.ApplyOpenEvent -= Teleport;
            BuilderManager.Instance.TestLevelEvent -= FindPrompt;
        }
        
        private void Teleport()
        {
             
        }

        private void FindPrompt()
        {
            prompt = FindObjectOfType<Prompt>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!BuilderManager.Instance.isMove || !isActive)
                return;
            
            if (other.GetComponentInParent<DroneController>() is DroneBuilderController drone)
            {
                _drone = drone.gameObject;
                prompt.PromptText = Idents.Tags.PromptText.PortalText;
                prompt.SetActive(true);
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if(!BuilderManager.Instance.isMove || !isActive)
                return;

            if (other.GetComponentInParent<DroneController>() is DroneBuilderController drone)
            {
                _drone = null;
                prompt.SetActive(false);
            }
        }

        public override void SetActive(bool active)
        {
            isActive = active;
        }

        public override void SetColorIndex(int color)
        {
            
        }
    }
}