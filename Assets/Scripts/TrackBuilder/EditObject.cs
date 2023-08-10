using System;
using System.Collections.Generic;
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
        
        private void Start()
        {
            HideEditMenu();
        }

        private readonly Dictionary<int, float> _sliderValues = new Dictionary<int, float>()
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
            var angleX = currentObject.Rotation.eulerAngles.x;
            var angleY = currentObject.Rotation.eulerAngles.y;
            var angleZ = currentObject.Rotation.eulerAngles.z;

            editMenu.SetEditPanelParams(currentObject.objectName,
                currentObject.Position.x, currentObject.Position.y, currentObject.Position.z, 
                angleX, angleY, angleZ,
                currentObject.Scale.x, currentObject);
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
            currentObject.Rotation = Quaternion.Euler(value, 0, 0);
        }

        public void OnYRotationChanged(float value)
        {
            currentObject.Rotation = Quaternion.Euler(currentObject.Rotation.eulerAngles.x, value, currentObject.Rotation.eulerAngles.z);
        }
        
        public void OnZRotationChanged(float value)
        {
            currentObject.Rotation = Quaternion.Euler(currentObject.Rotation.eulerAngles.x, currentObject.Rotation.eulerAngles.y, value);
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
            currentObject.interactiveObject.windMillRotateSpeed = value;
        }
        
        public void OnMagnetForceChanged(float value)
        {
            currentObject.interactiveObject.magnetForce = value;
        }

        public void OnPendulumSpeedChanged(float value)
        {
            currentObject.interactiveObject.pendulumMoveSpeed = value;
        }

        public void OnPendulumAngleChanged(float value)
        {
            currentObject.interactiveObject.rightPendulumAngle = value;
            currentObject.interactiveObject.leftPendulumAngle = -value;
        }

        public void OnWindZoneForceChanged(float value)
        {
            currentObject.interactiveObject.windForce = value;
        }

        public void OnBatteryEnergyChanged(float value)
        {
            currentObject.interactiveObject.batteryEnergy = value;
        }

        public void OnBoostChanged(float value)
        {
            currentObject.interactiveObject.boostSpeed = value;
        }

        public void OnHintTextChanged(string value)
        {
            if (currentObject.interactiveObject is TextWriter3D text3D)
            {
                text3D.Text = value;
            }
            else
            {
                currentObject.interactiveObject.hintText.text = value;
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
            if (currentObject.interactiveObject is ControllerPanel ControllerPanel)
            {
                ControllerPanel.set_color_index(value); 
            }
        }

        public void OnControllerPanelHackChanged(bool value)
        {
            if (currentObject.interactiveObject is ControllerPanel ControllerPanel)
            {
                ControllerPanel.set_hacked();
            }
        }

        public void OnControllerPanelHackN1Changed(int value)
        {
            if (currentObject.interactiveObject is ControllerPanel ControllerPanel)
            {
                ControllerPanel.set_code_n1(value); 
            }
        }

        public void OnControllerPanelHackN2Changed(int value)
        {
            if (currentObject.interactiveObject is ControllerPanel ControllerPanel)
            {
                ControllerPanel.set_code_n2(value); 
            }
        }

        public void OnControllerPanelHackN3Changed(int value)
        {
            if (currentObject.interactiveObject is ControllerPanel ControllerPanel)
            {
                ControllerPanel.set_code_n3(value); 
            }
        }

        public void OnControllerButtonColorChanged(int value)
        {
            if (currentObject.interactiveObject is ControllerButton ControllerButton)
            {
                ControllerButton.set_color_index(value); 
            }
        }

        public void OnControllerButtonDelayChanged(float value)
        {
            if (currentObject.interactiveObject is ControllerButton ControllerButton)
            {
                ControllerButton.set_time_value(value); 
            }
        }
    }
}