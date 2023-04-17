using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Menu
{
    public class MenuSettingsSpriteSwitcher : MonoBehaviour, IPointerClickHandler
    {
        public Image objectImage;
        public Sprite defaultSprite;
        public Sprite pressedSprite;

        public event Action<MenuSettingsSpriteSwitcher> OnButtonPressed;

        public void OnPointerClick(PointerEventData eventData)
        {
            // objectImage.sprite = pressedSprite;
            OnButtonPressed?.Invoke(this);
        }

        public void UpdateSprite(bool isPressed)
        {
            objectImage.sprite = isPressed ? pressedSprite : defaultSprite;
        }
    }
}