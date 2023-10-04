using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Drone.Menu
{
    public class SpriteSwapper : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
    {
        public Button button;
        public Image objectImage;
        public Sprite defaultSprite;
        public Sprite hoverSprite;
        public Sprite pressedSprite;
        public Sprite disabledSprite;
        public ColorSwapper colorSwapper;

        private void Awake()
        {
            objectImage = GetComponent<Image>();
            button = GetComponent<Button>();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            objectImage.sprite = defaultSprite;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(hoverSprite == null)
                return;
            
            objectImage.sprite = hoverSprite;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(pressedSprite == null)
                return;
            
            objectImage.sprite = pressedSprite;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            objectImage.sprite = defaultSprite;
            if(colorSwapper != null)
                colorSwapper.OnPointerExit(eventData);
        }
    }
}