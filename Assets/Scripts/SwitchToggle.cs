using System;
using UnityEngine;
using UnityEngine.UI;

namespace Drone
{
    public sealed class SwitchToggle : MonoBehaviour
    {
        [SerializeField] private Image targetImage;
        [SerializeField] private Sprite switchOn;
        [SerializeField] private Sprite switchOff;

        private void Start()
        {
            targetImage.sprite = GetComponent<Toggle>().isOn ? switchOn : switchOff;
        }

        public void Switch(bool value)
        {
            targetImage.sprite = value ? switchOn : switchOff;
        }
    }
}