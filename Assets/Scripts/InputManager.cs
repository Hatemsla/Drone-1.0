using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Drone
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance;
        public Vector3 mousePosition;
        public event Action FlashlightEvent;
        public event Action CopyObjectEvent;
        public event Action PasteObjectEvent;
        public event Action UndoObjectEvent;
        public event Action RedoObjectEvent;
        public event Action PlaceObjectEvent;
        public event Action PlaceAndPickupObjectEvent;
        public event Action SelectObjectEvent;
        public event Action SelectObjectsEvent;
        public event Action DeleteObjectEvent;
        public event Action<float> RotateYObjectEvent;
        public event Action<float> RotateXObjectEvent;
        public event Action<float> PedalsEvent;
        public event Action<float> ThrottleEvent;
        public event Action<Vector2> CyclicEvent;
        public event Action<bool> RewindTimeEvent;
        public event Action ThermalVisionEvent;
        public event Action XRayEvent;
        public event Action<float> ChangeObjectHeightEvent;
        public event Action<float> ChangeObjectScaleEvent;
        public event Action ExitEvent;
        public event Action SetCursorEvent;
        public event Action CameraBoostEvent;
        public event Action<Vector2> CameraMoveEvent;
        public event Action<float> CameraChangeHeightEvent;
        public event Action<float> CameraZoomEvent;
        public event Action LockCursorEvent;
        public event Action SwitchViewEvent;
        public event Action JerkEvent;
        public event Action ShieldEvent;
        public event Action ApplyOpenEvent;
        public event Action NextCameraEvent;
        public event Action PreviousCameraEvent;
        public event Action ExitPortEvent;

        public static event Action RebindComplete;
        public static event Action RebindCanceled;
        public static event Action<InputAction, int> RebindStarted;

        private static PlayerInputs _playerInput;
        private bool _isPlayerAction;

        private void Awake()
        {
            Instance = this;
            
            _playerInput ??= new PlayerInputs();

            _playerInput.Player.FlashLight.performed += _ => FlashlightEvent?.Invoke();
            _playerInput.Player.Throttle.performed += _ => ThrottleEvent?.Invoke(_.ReadValue<float>());
            _playerInput.Player.Throttle.canceled += _ => ThrottleEvent?.Invoke(0f);
            _playerInput.Player.Pedals.performed += _ => PedalsEvent?.Invoke(_.ReadValue<float>());
            _playerInput.Player.Pedals.canceled += _ => PedalsEvent?.Invoke(0f);
            _playerInput.Player.Cyclic.performed += _ => CyclicEvent?.Invoke(_.ReadValue<Vector2>());;
            _playerInput.Player.Cyclic.canceled += _ => CyclicEvent?.Invoke(Vector2.zero);
            _playerInput.Player.RewindTime.performed += _ => RewindTimeEvent?.Invoke(true);
            _playerInput.Player.RewindTime.canceled += _ => RewindTimeEvent?.Invoke(false);
            _playerInput.Player.ThermalVision.performed += _ => ThermalVisionEvent?.Invoke();
            _playerInput.Player.XRay.performed += _ => XRayEvent?.Invoke();
            _playerInput.Player.Exit.performed += _ => ExitEvent?.Invoke();
            _playerInput.Player.SwitchView.performed += _ => SwitchViewEvent?.Invoke();
            _playerInput.Player.Jerk.performed += _ => JerkEvent?.Invoke();
            _playerInput.Player.Shield.performed += _ => ShieldEvent?.Invoke();
            _playerInput.Player.ApplyOpen.performed += _ => ApplyOpenEvent?.Invoke();
            
            _playerInput.Port.NextCamera.performed += _ => NextCameraEvent?.Invoke();
            _playerInput.Port.PreviousCamera.performed += _ => PreviousCameraEvent?.Invoke();
            _playerInput.Port.ExitPort.performed += _ => ExitPortEvent?.Invoke();

            _playerInput.Builder.CopyObject.performed += _ => CopyObjectEvent?.Invoke();
            _playerInput.Builder.PasteObject.performed += _ => PasteObjectEvent?.Invoke();
            _playerInput.Builder.UndoCommand.performed += _ => UndoObjectEvent?.Invoke();
            _playerInput.Builder.RedoCommand.performed += _ => RedoObjectEvent?.Invoke();
            _playerInput.Builder.PlaceObject.performed += _ => PlaceObjectEvent?.Invoke();
            _playerInput.Builder.PlaceAndPickupSameObejct.performed += _ => PlaceAndPickupObjectEvent?.Invoke();
            _playerInput.Builder.SelectObject.performed += _ => SelectObjectEvent?.Invoke();
            _playerInput.Builder.SelectObjects.performed += _ => SelectObjectsEvent?.Invoke();
            _playerInput.Builder.DeleteObject.performed += _ => DeleteObjectEvent?.Invoke();
            _playerInput.Builder.RotateYObject.performed += _ => RotateYObjectEvent?.Invoke(_.ReadValue<float>());
            _playerInput.Builder.MouseScroll.performed += _ => RotateYObjectEvent?.Invoke(_.ReadValue<float>());
            _playerInput.Builder.RotateXObject.performed += _ => RotateXObjectEvent?.Invoke(_.ReadValue<float>());
            _playerInput.Builder.ChangeObjectHeight.performed += _ => ChangeObjectHeightEvent?.Invoke(_.ReadValue<float>());
            _playerInput.Builder.ChangeObjectHeight.canceled += _ => ChangeObjectHeightEvent?.Invoke(0f);
            _playerInput.Builder.ChangeObjectScale.performed += _ => ChangeObjectScaleEvent?.Invoke(_.ReadValue<float>());
            _playerInput.Builder.LockCursor.performed += _ => LockCursorEvent?.Invoke();
            
            _playerInput.Camera.SetCursor.performed += _ => SetCursorEvent?.Invoke();
            _playerInput.Camera.CameraBoostSpeed.performed += _ => CameraBoostEvent?.Invoke();
            _playerInput.Camera.CameraMove.performed += _ => CameraMoveEvent?.Invoke(_.ReadValue<Vector2>());
            _playerInput.Camera.CameraMove.canceled += _ => CameraMoveEvent?.Invoke(Vector2.zero);
            _playerInput.Camera.CameraChangeHeight.performed += _ => CameraChangeHeightEvent?.Invoke(_.ReadValue<float>());
            _playerInput.Camera.CameraChangeHeight.canceled += _ => CameraChangeHeightEvent?.Invoke(0);
            _playerInput.Camera.MouseScroll.performed += _ => CameraZoomEvent?.Invoke(_.ReadValue<float>());
            _playerInput.Camera.MouseScroll.canceled += _ => CameraZoomEvent?.Invoke(0f);
            _playerInput.Camera.MouseScroll.performed += _ => RotateXObjectEvent?.Invoke(_.ReadValue<float>());
            _playerInput.Camera.LockCursor.performed += _ => LockCursorEvent?.Invoke();
        }

        private void Update()
        {
            mousePosition = Mouse.current.position.ReadValue();
        }

        private void OnEnable()
        {
            _playerInput.Enable();
            _playerInput.Player.Disable();
            _playerInput.Builder.Disable();
            _playerInput.UI.Enable();
            _playerInput.Camera.Disable();
        }

        /// <summary>
        /// Action map logs
        /// </summary>
        /// <returns></returns>
        public string EnabledMaps()
        {
            return
                $"Player: {_playerInput.Player.enabled} Builder: {_playerInput.Builder.enabled} UI: {_playerInput.UI.enabled} Camera: {_playerInput.Camera.enabled}";
        }

        private void OnDisable()
        {
            _playerInput.Disable();
        }

        public void TurnCustomActionMap(string actionName)
        {
            foreach (var action in _playerInput.asset.actionMaps)
            {
                if(action.name == actionName)
                    action.Enable();
                else
                    action.Disable();
            }
        }

        public void TurnActionMaps()
        {
            _isPlayerAction = !_isPlayerAction;

            if (_isPlayerAction)
            {
                _playerInput.Player.Enable();
                _playerInput.Builder.Disable();
            }
            else
            {
                _playerInput.Player.Disable();
                _playerInput.Builder.Enable();
            }
        }

        public void TurnCameraActionMap()
        {
            if (_playerInput.Camera.enabled)
            {
                _playerInput.Camera.Disable();
                _playerInput.Builder.Enable();
            }
            else
            {
                _playerInput.Camera.Enable();
                _playerInput.Builder.Disable();
            }
        }

        public void StartRebind(string actionName, int bindingIndex, TMP_Text statusText, bool excludeMouse)
        {
            var action = _playerInput.asset.FindAction(actionName);
            if (action == null || action.bindings.Count <= bindingIndex)
            {
                Debug.Log("Couldn't find action or binding");
                return;
            }

            if (action.bindings[bindingIndex].isComposite)
            {
                var firstPartIndex = bindingIndex + 1;
                if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isComposite)
                    DoRebind(action, bindingIndex, statusText, true, excludeMouse);
            }
            else
                DoRebind(action, bindingIndex, statusText, false, excludeMouse);
        }

        private void DoRebind(InputAction actionToRebind, int bindingIndex, TMP_Text statusText, bool allCompositeParts, bool excludeMouse)
        {
            if (actionToRebind == null || bindingIndex < 0)
                return;

            statusText.text = $"Нажмите на кнопку";

            actionToRebind.Disable();

            var rebind = actionToRebind.PerformInteractiveRebinding(bindingIndex);

            rebind.OnComplete(operation =>
            {
                actionToRebind.Enable();
                operation.Dispose();

                if(allCompositeParts)
                {
                    var nextBindingIndex = bindingIndex + 1;
                    if (nextBindingIndex < actionToRebind.bindings.Count && actionToRebind.bindings[nextBindingIndex].isComposite)
                        DoRebind(actionToRebind, nextBindingIndex, statusText, allCompositeParts, excludeMouse);
                }

                SaveBindingOverride(actionToRebind);
                RebindComplete?.Invoke();
            });

            rebind.OnCancel(operation =>
            {
                actionToRebind.Enable();
                operation.Dispose();

                RebindCanceled?.Invoke();
            });

            rebind.WithCancelingThrough("<Keyboard>/escape");

            if (excludeMouse)
                rebind.WithControlsExcluding("Mouse");

            RebindStarted?.Invoke(actionToRebind, bindingIndex);
            rebind.Start();
        }
        
        private static void SaveBindingOverride(InputAction action)
        {
            for (int i = 0; i < action.bindings.Count; i++)
            {
                PlayerPrefs.SetString(action.actionMap + action.name + i, action.bindings[i].overridePath);
            }
        }

        public void LoadBindingOverride(string actionName)
        {
            if (_playerInput == null)
                _playerInput = new PlayerInputs();

            InputAction action = _playerInput.asset.FindAction(actionName);

            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (!string.IsNullOrEmpty(PlayerPrefs.GetString(action.actionMap + action.name + i)))
                    action.ApplyBindingOverride(i, PlayerPrefs.GetString(action.actionMap + action.name + i));
            }
        }

        public void ResetBinding(string actionName, int bindingIndex)
        {
            InputAction action = _playerInput.asset.FindAction(actionName);

            if(action == null || action.bindings.Count <= bindingIndex)
            {
                Debug.Log("Could not find action or binding");
                return;
            }

            if (action.bindings[bindingIndex].isComposite)
            {
                for (int i = bindingIndex; i < action.bindings.Count && action.bindings[i].isComposite; i++)
                    action.RemoveBindingOverride(i);
            }
            else
                action.RemoveBindingOverride(bindingIndex);

            SaveBindingOverride(action);
        }
        
        
        public static string GetBindingName(string actionName, int bindingIndex)
        {
            _playerInput ??= new PlayerInputs();

            var action = _playerInput.asset.FindAction(actionName);
            return action.GetBindingDisplayString(bindingIndex);
        }
    }
}