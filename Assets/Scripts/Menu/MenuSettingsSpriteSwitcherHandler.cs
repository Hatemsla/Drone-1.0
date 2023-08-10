using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Drone.Menu
{
    public class MenuSettingsSpriteSwitcherHandler : MonoBehaviour
    {
        public List<MenuSettingsSpriteSwitcher> buttons;

        private void Start()
        {
            foreach (var button in buttons)
            {
                button.OnButtonPressed += OnButtonPressed;
            }

            buttons[0].UpdateSprite(true);
        }

        private void OnButtonPressed(MenuSettingsSpriteSwitcher pressedButton)
        {
            foreach (var button in buttons)
            {
                button.UpdateSprite(button == pressedButton);
            }
        }
    }
}