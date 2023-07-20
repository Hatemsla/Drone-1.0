using System;
using System.Collections.Generic;
using Builder;
using Drone.SerializeDictionaries;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Drone
{
    public sealed class PortControl : SerializableDictionary<ControlIcons, Sprite>
    {
        [SerializeField] private TMP_Text controlName;
        [SerializeField] private GameObject connectionText;
        [SerializeField] private Image controlImage;
        [SerializeField] private Sprite onConnectionSprite;
        [SerializeField] private Sprite offConnectionSprite;
        [SerializeField] private Sprite onSliderSprite;
        [SerializeField] private Sprite offSliderSprite;
        [SerializeField] private PortObjectBackground portObjectBackground;
        [SerializeField] private ControlSprites controlSprites;

        public List<InteractiveObject> controlObjects;

        public void AddControlObjects(List<InteractiveObject> controlObjs, Port port, InteractiveType interactiveType)
        {
            controlObjects = controlObjs;
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
            }
        }
    }
}