using System;
using System.Collections.Generic;
using System.Linq;
using Drone;
using UnityEngine;

namespace Builder
{
    public sealed class Port : MonoBehaviour
    {
        [SerializeField] private Prompt prompt;
        public bool isCameraChange;
        public List<SecurityCamera> securityCameras;
        public List<Lamp> lamps;
        public List<InteractiveObject> interactiveObjects;

        public event Action<Port> PortOpenEvent;
        public event Action PortCloseEvent;

        private int _currentCameraIndex;
        private int _mainCameraIndex;
        private bool _previousIsMove;

        private void OnEnable()
        {
            InputManager.Instance.ApplyOpenEvent += OpenPort;
            InputManager.Instance.NextCameraEvent += NextCameraActivation;
            InputManager.Instance.PreviousCameraEvent += PreviousCameraActivation;
            InputManager.Instance.ExitPortEvent += ClosePort;
            InputManager.Instance.ExitPortEvent += DeactivateAllCameras;
            BuilderManager.Instance.TestLevelEvent += GetAllSecurityCameras;
            BuilderManager.Instance.ObjectChangeSceneEvent += FindPrompt;
        }

        private void OnDisable()
        {
            InputManager.Instance.ApplyOpenEvent -= OpenPort;
            InputManager.Instance.NextCameraEvent -= NextCameraActivation;
            InputManager.Instance.PreviousCameraEvent -= PreviousCameraActivation;
            InputManager.Instance.ExitPortEvent -= ClosePort;
            InputManager.Instance.ExitPortEvent -= DeactivateAllCameras;
            BuilderManager.Instance.TestLevelEvent -= GetAllSecurityCameras;
            BuilderManager.Instance.ObjectChangeSceneEvent -= FindPrompt;
        }

        private void FindPrompt()
        {
            prompt = FindObjectOfType<BuilderUI>().prompt;
        }

        private void GetAllSecurityCameras()
        {
            if (!BuilderManager.Instance.isMove)
            {
                prompt.SetActive(false);
                return;
            }

            interactiveObjects = FindObjectsOfType<InteractiveObject>().ToList();
            foreach (var interactive in interactiveObjects)
            {
                switch (interactive)
                {
                    case SecurityCamera cam:
                        securityCameras.Add(cam);
                        break;
                    case Lamp lamp:
                        lamps.Add(lamp);
                        break;
                }
            }
            Debug.Log("Найдено " + securityCameras.Count + " камер(ы) безопасности.");   
        }
        
        private void OpenPort()
        {
            if (!prompt.gameObject.activeSelf)
                return;
            
            PortOpenEvent?.Invoke(this);
            InputManager.Instance.TurnCustomActionMap("Port");
            BuilderManager.Instance.builderUI.droneView.SetActive(false);
            BuilderManager.Instance.builderUI.objectEditPanel.SetActive(false);
            BuilderManager.Instance.builderUI.portUI.SetActive(true);
            prompt.SetActive(false);
        }
        
        private void ClosePort()
        {
            if(isCameraChange)
                return;
            
            PortCloseEvent?.Invoke();
            InputManager.Instance.TurnCustomActionMap("Player");
            BuilderManager.Instance.builderUI.droneView.SetActive(true);
            BuilderManager.Instance.builderUI.portUI.SetActive(false);
        }

        public void ActivateSecurityCameras()
        {
            isCameraChange = true;
            NextCameraActivation();
        }
        
        private void NextCameraActivation()
        {
            if (securityCameras.Count < 1 || !isCameraChange)
                return;

            CheckCameraCurrentIndex();
            
            for (var i = 0; i < securityCameras.Count; i++)
            {
                securityCameras[i].SetPriority(_currentCameraIndex == i ? 11 : 0);
            }
            
            _currentCameraIndex++;
        }

        private void PreviousCameraActivation()
        {
            if (securityCameras.Count < 1 || !isCameraChange)
                return;
            
            CheckCameraCurrentIndex();

            for (var i = 0; i < securityCameras.Count; i++)
            {
                securityCameras[i].SetPriority(_currentCameraIndex == i ? 11 : 0);
            }
            
            _currentCameraIndex--;
        }

        private void CheckCameraCurrentIndex()
        {
            if (_currentCameraIndex < 0)
                _currentCameraIndex = securityCameras.Count - 1;

            if (_currentCameraIndex >= securityCameras.Count)
                _currentCameraIndex = 0;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!BuilderManager.Instance.isMove)
                return;
            
            if (other.GetComponentInParent<DroneController>() is DroneBuilderController drone)
            {
                prompt.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if(!BuilderManager.Instance.isMove)
                return;

            if (other.GetComponentInParent<DroneController>() is DroneBuilderController drone)
            {
                prompt.SetActive(false);
            }
        }

        private void DeactivateAllCameras()
        {
            if(!isCameraChange)
                return;
            
            foreach (var camera in securityCameras) camera.SetPriority(0);
            BuilderManager.Instance.builderUI.portUI.SetActive(true);
            isCameraChange = false;
        }

        public List<List<InteractiveObject>> GetAllInteractiveObjects()
        {
            var result = new List<List<InteractiveObject>>
            {
                new(securityCameras),
                new(lamps)
            };

            return result;
        }
    }
}