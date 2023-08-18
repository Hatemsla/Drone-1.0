using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Drone.Builder
{
    public sealed class PortalObject : InteractiveObject
    {
        [SerializeField] private string map;
        [SerializeField] private Prompt prompt;

        private GameObject _drone;
        private bool _isTrigger;
        
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
            if(!_isTrigger)
                return;
            
            GameManager.Instance.gameData.levelName = map;
            GameManager.Instance.gameData.isLoadLevel = false;
            GameManager.Instance.gameData.isStartBuilder = true;
            SceneManager.LoadScene(4);
        }

        public void SetMap(string newMap) => map = newMap;
        public string GetMap() => map;

        private void FindPrompt()
        {
            prompt = BuilderManager.Instance.builderUI.prompt;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!BuilderManager.Instance.isMove || !isActive)
                return;
            
            
            if (other.GetComponentInParent<DroneController>() is DroneBuilderController drone)
            {
                if (!LevelManager.IsLevelExist(map))
                {
                    prompt.PromptText = Idents.Tags.PromptText.NoPortalText;
                    prompt.SetActive(true);
                }
                else
                {
                    _drone = drone.gameObject;
                    prompt.PromptText = Idents.Tags.PromptText.PortalText;
                    prompt.SetActive(true);
                    _isTrigger = true;   
                }
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
                _isTrigger = false;
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