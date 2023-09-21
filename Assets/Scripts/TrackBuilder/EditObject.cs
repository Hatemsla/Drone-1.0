using System;
using System.Collections.Generic;
using System.Linq;
using Drone.Builder.Text3D;
using Drone.Builder.ControllerElements;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Drone.Builder
{
    public class EditObject : MonoBehaviour
    {
        public TrackObject currentObject;
        [SerializeField] private BuilderUI builderUI;
        [SerializeField] private EditMenu editMenu;

        private List<string> _maps;
        private List<string> _sounds;
        
        private readonly Dictionary<int, float> _sliderValues = new()
        {
            { 0, 0.5f },
            { 1, 1f },
            { 2, 1.5f },
            { 3, 2f },
            { 4, 2.5f },
            { 5, 3f },
            { 6, 3.5f },
            { 7, 4f },
            { 8, 4.5f },
            { 9, 5f },
            { 10, 5.5f },
            { 11, 6f },
            { 12, 6.5f },
            { 13, 7f },
            { 14, 7.5f },
            { 15, 8f },
        };
        
        private void Start()
        {
            HideEditMenu();
            GetMaps();
            GetSounds();
        }

        private void GetMaps()
        {
            _maps = LevelManager.LoadMaps().ToList();
            _maps.Insert(0, "No map");
        }

        private void GetSounds()
        {
            _sounds = TrackBuilderUtils.LoadSounds().ToList();
            _sounds.Insert(0, "No sound");
        }
        
        public void ShowEditMenu()
        {
            editMenu.gameObject.SetActive(true);
        }

        public void HideEditMenu()
        {
            editMenu.gameObject.SetActive(false);
        }
        
        public void OnSelectObject(TrackObject obj)
        {
            currentObject = obj;
            editMenu.SetEditPanelParams(currentObject);
        }

        public void OnXPositionChanged(string value)
        {
            if(float.TryParse(value, out var x))
                currentObject.Position = new Vector3(x, currentObject.Position.y, currentObject.Position.z);
        }
        
        public void OnYPositionChanged(string value)
        {
            if(float.TryParse(value, out var y))
                currentObject.Position = new Vector3(currentObject.Position.x, y, currentObject.Position.z);
        }
        
        public void OnZPositionChanged(string value)
        {
            if(float.TryParse(value, out var z))
                currentObject.Position = new Vector3(currentObject.Position.x, currentObject.Position.y, z);
        }
        
        public void OnXRotationChanged(float value)
        {
            currentObject.transform.Rotate(value - currentObject.previousRotationXValue, 0, 0, Space.World);
            currentObject.previousRotationXValue = value;
        }

        public void OnXRotationTextChanged(string text)
        {
            var currentRotation = currentObject.Rotation.eulerAngles;
            var xRot = float.Parse(text);
            xRot = xRot switch
            {
                > 180f => 180f,
                < 0f => 0f,
                _ => xRot
            };
            currentRotation.x = xRot;
            currentObject.Rotation = Quaternion.Euler(currentRotation);
        }

        public void OnYRotationChanged(float value)
        {
            var currentRotation = currentObject.Rotation.eulerAngles;
            currentRotation.y = value;
            currentObject.Rotation = Quaternion.Euler(currentRotation);
        }

        public void OnZRotationChanged(float value)
        {
            var currentRotation = currentObject.Rotation.eulerAngles;
            currentRotation.z = value;
            currentObject.Rotation = Quaternion.Euler(currentRotation);
        }

        public void OnXYZScaleChanged(float value)
        {
            if(currentObject.objectType is ObjectsType.Drone or ObjectsType.Gate)
                return;

            var newScale = _sliderValues[(int)value];
            var oldScale = currentObject.Scale.x;
            currentObject.Scale = new Vector3(newScale, newScale, newScale);

            if (newScale < oldScale)
            {
                var currentObjectYOffset = (oldScale - newScale);
                currentObject.Position = new Vector3(currentObject.Position.x,
                    currentObject.Position.y - currentObject.yOffsetIncrease, currentObject.Position.z);
                currentObject.yOffset -= currentObject.yOffsetIncrease;
                currentObject.maxMouseDistance -= currentObjectYOffset;
            }
            else if (newScale > oldScale)
            {
                var currentObjectYOffset = (newScale - oldScale);
                currentObject.Position = new Vector3(currentObject.Position.x,
                    currentObject.Position.y + currentObject.yOffsetIncrease, currentObject.Position.z);
                currentObject.yOffset += currentObject.yOffsetIncrease;
                currentObject.maxMouseDistance += currentObjectYOffset;
            }
        }

        public void OnWindmillRotationSpeedChanged(float value)
        {
            ((Windmill)currentObject.interactiveObject).windMillRotateSpeed = value;
        }
        
        public void OnMagnetForceChanged(float value)
        {
            if(currentObject.interactiveObject is MagnetKiller magnetKiller)
                magnetKiller.magnetForce = value;
            else if (currentObject.interactiveObject is RigidbodyMagnet rigidbodyMagnet)
                rigidbodyMagnet.magnetForce = value;
        }

        public void OnMagnetKillerRotateSpeedChanged(float value)
        {
            ((MagnetKiller)currentObject.interactiveObject).rotationSpeed = value;
        }
        
        public void OnMagnetKillerDamageChanged(float value)
        {
            ((MagnetKiller)currentObject.interactiveObject).baseDamage = value;
        }
        
        public void OnMagnetKillerDamageIntervalChanged(float value)
        {
            ((MagnetKiller)currentObject.interactiveObject).damageInterval = value;
        }

        public void OnPendulumSpeedChanged(float value)
        {
            ((Pendulum)currentObject.interactiveObject).pendulumMoveSpeed = value;
        }

        public void OnPendulumAngleChanged(float value)
        {
            var pendulum = (Pendulum)currentObject.interactiveObject;
            pendulum.rightPendulumAngle = value;
            pendulum.leftPendulumAngle = -value;
        }

        public void OnWindZoneForceChanged(float value)
        {
            ((WindZoneScript)currentObject.interactiveObject).windForce = value;
        }

        public void OnBatteryEnergyChanged(float value)
        {
            ((Battery)currentObject.interactiveObject).batteryEnergy = value;
        }

        public void OnBoostChanged(float value)
        {
            ((BoostTrigger)currentObject.interactiveObject).boostSpeed = value;
        }

        public void OnHintTextChanged(string value)
        {
            if (currentObject.interactiveObject is TextWriter3D text3D)
            {
                text3D.Text = value;
            }
            else if (currentObject.interactiveObject is TriggerMessage triggerMassege)
            {
                triggerMassege.triggerText = value;
            }
            else
            {
                ((Hint)currentObject.interactiveObject).hintText.text = value;
            }
        }

        public void OnSelectTextHint(string value)
        {
            InputSystem.DisableDevice(Keyboard.current);
        }
        
        public void OnDeselectTextHint(string value)
        {
            InputSystem.EnableDevice(Keyboard.current);
        }

        public void OnStartDrawButton()
        {
            if (currentObject.interactiveObject is DrawLine drawLine)
            {
                drawLine.StartDraw();
                builderUI.editButtons.SetActive(false);
                builderUI.createPanel.SetActive(false);
            }
        }
        
        public void OnStopDrawButton()
        {
            if (currentObject.interactiveObject is DrawLine drawLine)
            {
                drawLine.StopDraw();
                builderUI.editButtons.SetActive(true);
                builderUI.createPanel.SetActive(true);
            }
        }

        public void OnThicknessChanged(float value)
        {
            if (currentObject.interactiveObject is DrawLine drawLine)
            {
                drawLine.SetThickness(value);
            }
        }
        
        public void OnColorChanged(float value)
        {
            if (currentObject.interactiveObject is DrawLine drawLine)
            {
                drawLine.SetColor(Color.HSVToRGB(value, 1f, 1f));
            }
        }

        public void OnSetActiveObject(bool value)
        {
            currentObject.interactiveObject.SetActive(value);
        }

        public void OnSetColorObject(int value)
        {
            currentObject.interactiveObject.SetColorIndex(value);
        }
        
        public void OnHasPasswordChanged(bool value)
        {
            if (currentObject.interactiveObject is Port port)
            {
                port.hasPassword = value;
            }
        }

        public void OnPasswordChanged(string password)
        {
            editMenu.PasswordHintActive(false);
            if (password.Length < 3)
            {
                editMenu.PasswordHintActive(true);
            }

            if (currentObject.interactiveObject is Port port)
            {
                port.portPassword.Password = password;
                port.hasPassword = password.Length >= 3;
            }
            else if (currentObject.interactiveObject is ControllerPanel controllerPanel)
            {
                controllerPanel.SetPassword(password, password.Length >= 3);
            }
        }

        public void OnElectroGateColorChanged(int value)
        {
            if (currentObject.interactiveObject is ControledGate controledGate)
            {
                controledGate.set_color_index(value); 
            }  
        }

        public void OnControllerPanelColorChanged(int value)
        {
            if (currentObject.interactiveObject is ControllerPanel controllerPanel)
            {
                controllerPanel.set_color_index(value); 
            }
        }

        public void OnControllerButtonColorChanged(int value)
        {
            if (currentObject.interactiveObject is ControllerButton controllerButton)
            {
                controllerButton.set_color_index(value); 
            }
        }

        public void OnControllerButtonDelayChanged(float value)
        {
            if (currentObject.interactiveObject is ControllerButton controllerButton)
            {
                controllerButton.set_time_value(value); 
            }
        }

        public void OnMapChanged(int value)
        {
            if (currentObject.interactiveObject is PortalObject portalObject)
            {
                portalObject.SetMap(_maps[value]);
            }
        }

        public void OnSetSoundObject(int value)
        {
            GetSounds();
            if (currentObject.interactiveObject is TriggerMessage trMessage)
            {
                trMessage.SetSound(_sounds[value]); 
            }
        }
    }
}