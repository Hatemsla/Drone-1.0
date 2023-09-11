using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Drone.Builder
{
    public sealed class PortalObject : InteractiveObject
    {
        [SerializeField] private string map;
        [SerializeField] private Prompt prompt;

        private DroneRpgController _droneRpgController;
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
            
            _droneRpgController.IsReset = false;
            _droneRpgController.TimeForEndGame = BuilderManager.Instance.timer.waitForEndGame;
            
            GameManager.Instance.gameData.levelName = map;
            BuilderManager.Instance.levelName = map;
            
            GameManager.Instance.gameData.isLoadLevel = false;
            GameManager.Instance.gameData.isStartBuilder = true;
            GameManager.Instance.gameData.isTeleportLevel = true;
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
                _droneRpgController = drone.GetComponent<DroneRpgController>();
                if (!LevelManager.IsLevelExist(map))
                {
                    prompt.PromptText = Idents.Tags.PromptText.NoPortalText;
                    prompt.SetActive(true);
                }
                else
                {
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
                _droneRpgController = null;
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