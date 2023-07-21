using System;
using System.Collections.Generic;
using Builder;
using Drone.SerializeDictionaries;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Drone
{
    public sealed class PortControl : MonoBehaviour
    {
        [SerializeField] private TMP_Text controlName;
        [SerializeField] private GameObject connectionText;
        [SerializeField] private Image controlImage;
        [SerializeField] private Sprite onConnectionSprite;
        [SerializeField] private Sprite offConnectionSprite;
        [SerializeField] private Sprite onSliderSprite;
        [SerializeField] private Sprite offSliderSprite;
        [SerializeField] private PortObjectBackground portObjectBackground;
        [SerializeField] private SwitchToggle switchToggle;
        [SerializeField] private ControlSprites controlSprites;

        private List<InteractiveObject> _controlObjects;

        private void OnEnable()
        {
            switchToggle.SwitchEvent += OnSwitch;
        }

        private void OnDisable()
        {
            switchToggle.SwitchEvent -= OnSwitch;
        }

        private void OnSwitch(bool value)
        {
            foreach (var controlObject in _controlObjects)
            {
                controlObject.SetActive(value);
            }
        }

        public void AddControlObjects(List<InteractiveObject> controlObjs, Port port, InteractiveType interactiveType)
        {
            switchToggle.Toggle(Utils.GetActive(controlObjs));
            _controlObjects = controlObjs;
            switch (interactiveType)
            {
                case InteractiveType.SecureCamera:
                    controlName.text = "Видеокамера";
                    connectionText.SetActive(true);
                    controlImage.sprite = controlSprites.dictionary[ControlIcons.Camera];
                    portObjectBackground.GetConnectionAction(port.ActivateSecurityCameras);
                    break;
                case InteractiveType.Lamp:
                    controlName.text = "Освещение";
                    connectionText.SetActive(false);
                    controlImage.sprite = controlSprites.dictionary[ControlIcons.Lamp];
                    portObjectBackground.GetConnectionAction(port.ActivateSecurityCameras);
                    break;
                case InteractiveType.Magnet:
                    controlName.text = "Магнит";
                    connectionText.SetActive(false);
                    controlImage.sprite = controlSprites.dictionary[ControlIcons.Magnet];
                    break;
                case InteractiveType.Pendulum:
                    controlName.text = "Булава";
                    connectionText.SetActive(false);
                    controlImage.sprite = controlSprites.dictionary[ControlIcons.Pendulum];
                    break;
                case InteractiveType.Battery:
                    controlName.text = "Батарейка";
                    connectionText.SetActive(false);
                    controlImage.sprite = controlSprites.dictionary[ControlIcons.Battery];
                    break;
                case InteractiveType.Windmill:
                    controlName.text = "Вентилятор";
                    connectionText.SetActive(false);
                    controlImage.sprite = controlSprites.dictionary[ControlIcons.Windmill];
                    break;
            }
        }
    }
}