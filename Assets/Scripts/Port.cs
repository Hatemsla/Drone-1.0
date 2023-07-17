using System.Collections.Generic;
using System.Linq;
using Drone;
using UnityEngine;

namespace Builder
{
    public class Port : MonoBehaviour
    {
        [SerializeField] private Prompt prompt;
        [SerializeField] private List<SecurityCamera> securityCameras;
        private int _currentCameraIndex;
        private int _mainCameraIndex;
        private bool _previousIsMove;

        private void OnEnable()
        {
            InputManager.Instance.ApplyOpenEvent += OpenPort;
            InputManager.Instance.NextCameraEvent += NextCameraActivation;
            InputManager.Instance.PreviousCameraEvent += PreviousCameraActivation;
            InputManager.Instance.ExitPortEvent += ClosePort;
            BuilderManager.Instance.TestLevelEvent += GetAllSecurityCameras;
            BuilderManager.Instance.ObjectChangeSceneEvent += FindPrompt;
        }

        private void OnDisable()
        {
            InputManager.Instance.ApplyOpenEvent -= OpenPort;
            InputManager.Instance.NextCameraEvent -= NextCameraActivation;
            InputManager.Instance.PreviousCameraEvent -= PreviousCameraActivation;
            InputManager.Instance.ExitPortEvent -= ClosePort;
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

            securityCameras = FindObjectsOfType<SecurityCamera>().ToList();   
            Debug.Log("Найдено " + securityCameras.Count + " камер(ы) безопасности.");   
        }
        
        private void OpenPort()
        {
            if (!prompt.gameObject.activeSelf)
                return;
            
            InputManager.Instance.TurnCustomActionMap("Port");
            BuilderManager.Instance.builderUI.droneView.SetActive(false);
            BuilderManager.Instance.builderUI.objectEditPanel.SetActive(false);
            prompt.SetActive(false);
            NextCameraActivation();
        }
        
        private void ClosePort()
        {
            foreach (var camera in securityCameras)
                camera.SetPriority(0);
            
            InputManager.Instance.TurnCustomActionMap("Player");
            BuilderManager.Instance.builderUI.droneView.SetActive(true);
        }
        
        private void NextCameraActivation()
        {
            if (securityCameras.Count < 1)
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
            if (securityCameras.Count < 1)
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
            foreach (var camera in securityCameras) camera.SetPriority(0);
        }
    }
}