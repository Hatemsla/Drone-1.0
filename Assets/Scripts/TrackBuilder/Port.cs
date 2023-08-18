using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Drone.Builder
{
    public sealed class Port : InteractiveObject
    {
        [SerializeField] private Prompt prompt;
        public PortPassword portPassword;
        public bool isOpen;
        public bool isCameraChange;
        
        public List<SecurityCamera> securityCameras;
        public List<Lamp> lamps;
        public List<Pendulum> pendulums;
        public List<MagnetKiller> magnetKillers;
        public List<RigidbodyMagnet> rigidbodyMagnets;
        public List<Windmill> windmills;
        public List<Battery> batteries;
        public List<FreezingBall> freezingBalls;
        public List<PitStop> pitStops;
        public List<InteractiveObject> interactiveObjects;

        public event Action<Port> PortOpenEvent;
        public event Action<int> ChangeSecurityCameraEvent;
        public event Action PortCloseEvent;

        private bool _inTrigger;
        private int _currentCameraIndex;
        private int _mainCameraIndex;
        private bool _previousIsMove;
        private bool _isTrigger;

        private void OnEnable()
        {
            InputManager.Instance.ApplyOpenEvent += OpenPort;
            InputManager.Instance.NextCameraEvent += NextCameraActivation;
            InputManager.Instance.PreviousCameraEvent += PreviousCameraActivation;
            InputManager.Instance.ExitPortEvent += ClosePort;
            InputManager.Instance.ExitPortEvent += DeactivateAllCameras;
            BuilderManager.Instance.TestLevelEvent += GetAllInteractiveObjects;
            BuilderManager.Instance.ObjectChangeSceneEvent += FindPrompt;
        }

        private void OnDisable()
        {
            InputManager.Instance.ApplyOpenEvent -= OpenPort;
            InputManager.Instance.NextCameraEvent -= NextCameraActivation;
            InputManager.Instance.PreviousCameraEvent -= PreviousCameraActivation;
            InputManager.Instance.ExitPortEvent -= ClosePort;
            InputManager.Instance.ExitPortEvent -= DeactivateAllCameras;
            BuilderManager.Instance.TestLevelEvent -= GetAllInteractiveObjects;
            BuilderManager.Instance.ObjectChangeSceneEvent -= FindPrompt;
        }

        private void FindPrompt()
        {
            prompt = BuilderManager.Instance.builderUI.prompt;
        }

        private void GetAllInteractiveObjects()
        {
            if (!BuilderManager.Instance.isMove)
            {
                prompt.SetActive(false);
                return;
            }

            interactiveObjects = FindObjectsOfType<InteractiveObject>().ToList();
            CleatInteractiveObjects();
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
                    case MagnetKiller magnetKiller:
                        magnetKillers.Add(magnetKiller);
                        break;
                    case Battery battery:
                        batteries.Add(battery);
                        break;
                    case RigidbodyMagnet magnet:
                        rigidbodyMagnets.Add(magnet);
                        break;
                    case Pendulum pendulum:
                        pendulums.Add(pendulum);
                        break;
                    case Windmill windmill:
                        windmills.Add(windmill);
                        break;
                    case FreezingBall freezingBall:
                        freezingBalls.Add(freezingBall);
                        break;
                    case PitStop pitStop:
                        pitStops.Add(pitStop);
                        break;
                }
            }
        }

        private void CleatInteractiveObjects()
        {
            securityCameras.Clear();
            lamps.Clear();
        }

        private void OpenPort()
        {
            if (!_inTrigger || !isActive || !_isTrigger)
                return;

            if (hasPassword)
            {
                portPassword.OpenPassword();
                return;
            }

            PortOpenEvent?.Invoke(this);
            isOpen = true;
            InputManager.Instance.TurnCustomActionMap("Port");
            BuilderManager.Instance.builderUI.droneView.SetActive(false);
            BuilderManager.Instance.builderUI.objectEditPanel.SetActive(false);
            BuilderManager.Instance.builderUI.portUI.SetActive(true);
            BuilderManager.Instance.builderUI.passwordUI.SetActive(false);
            prompt.SetActive(false);
        }

        public void OpenPortAfterPassword()
        {
            hasPassword = false;
            PortOpenEvent?.Invoke(this);
            isOpen = true;
            InputManager.Instance.TurnCustomActionMap("Port");
            BuilderManager.Instance.builderUI.droneView.SetActive(false);
            BuilderManager.Instance.builderUI.objectEditPanel.SetActive(false);
            BuilderManager.Instance.builderUI.portUI.SetActive(true);
            BuilderManager.Instance.builderUI.passwordUI.SetActive(false);
            prompt.SetActive(false);
        }

        private void ClosePort()
        {
            if(isCameraChange || !isOpen || !_isTrigger)
                return;
            
            PortCloseEvent?.Invoke();
            isOpen = false;
            InputManager.Instance.TurnCustomActionMap("Player");
            BuilderManager.Instance.builderUI.droneView.SetActive(true);
            BuilderManager.Instance.builderUI.portUI.SetActive(false);
            BuilderManager.Instance.builderUI.passwordUI.SetActive(false);
        }

        public void ActivateSecurityCameras()
        {
            if(!TrackBuilderUtils.GetActive(securityCameras))
                return;
            
            isCameraChange = true;
            BuilderManager.Instance.builderUI.portUI.SetActive(false);
            BuilderManager.Instance.builderUI.securityCameraView.SetActive(true);
            NextCameraActivation();
        }
        
        private void NextCameraActivation()
        {
            if (securityCameras.Count < 1 || !isCameraChange)
                return;
            
            CheckCameraCurrentIndex();

            var isCameraUpdate = false;
            for (var i = 0; i < securityCameras.Count; i++)
            {
                isCameraUpdate = securityCameras[i].SetPriority(_currentCameraIndex == i ? 11 : 0);
            }
            
            if(!isCameraUpdate)
                return;
            
            ChangeSecurityCameraEvent?.Invoke(_currentCameraIndex+1);
            
            _currentCameraIndex++;
        }

        private void PreviousCameraActivation()
        {
            if (securityCameras.Count < 1 || !isCameraChange)
                return;
            
            CheckCameraCurrentIndex();

            var isCameraUpdate = false;
            for (var i = 0; i < securityCameras.Count; i++)
            {
                isCameraUpdate = securityCameras[i].SetPriority(_currentCameraIndex == i ? 11 : 0);
            }
            
            if(!isCameraUpdate)
                return;
            
            ChangeSecurityCameraEvent?.Invoke(_currentCameraIndex+1);
            
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
            if(!BuilderManager.Instance.isMove || !isActive)
                return;
            
            if (other.GetComponentInParent<DroneController>() is DroneBuilderController drone)
            {
                prompt.PromptText = Idents.Tags.PromptText.DefaultText;
                prompt.SetActive(true);
                _isTrigger = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if(!BuilderManager.Instance.isMove || !isActive)
                return;

            if (other.GetComponentInParent<DroneController>() is DroneBuilderController drone)
            {
                DeactivateAllCameras();
                ClosePort();
                prompt.SetActive(false);
                _isTrigger = false;
            }
        }

        private void DeactivateAllCameras()
        {
            if(!isCameraChange)
                return;
            
            foreach (var securityCamera in securityCameras) securityCamera.SetPriority(0);
            BuilderManager.Instance.builderUI.portUI.SetActive(true);
            BuilderManager.Instance.builderUI.securityCameraView.SetActive(false);
            isCameraChange = false;
        }

        public List<List<InteractiveObject>> ReturnAllInteractiveObjects()
        {
            var result = new List<List<InteractiveObject>>
            {
                new(securityCameras),
                new(lamps),
                new(batteries),
                new(magnetKillers),
                new(rigidbodyMagnets),
                new(pendulums),
                new(windmills),
                new(freezingBalls),
                new(pitStops),
            };

            return result;
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