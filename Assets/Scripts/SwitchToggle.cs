using System;
using UnityEngine;
using UnityEngine.UI;

namespace Drone
{
    public sealed class SwitchToggle : MonoBehaviour
    {
        [SerializeField] private Image targetImage;
        [SerializeField] private Toggle toggle;
        [SerializeField] private Sprite switchOn;
        [SerializeField] private Sprite switchOff;

        public event Action<bool> SwitchEvent;
        
        private void Start()
        {
            targetImage.sprite = GetComponent<Toggle>().isOn ? switchOn : switchOff;
        }

        private void Switch(bool value)
        {
            SwitchEvent?.Invoke(value);
            targetImage.sprite = value ? switchOn : switchOff;
        }

        public void Toggle(bool value)
        {
            toggle.isOn = value;
        }
    }
}