using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Menu
{
    public class SpriteSwapper : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
    {
        public Image objectImage;
        public Sprite defaultSprite;
        public Sprite hoverSprite;
        public Sprite pressedSprite;
        public Sprite disabledSprite;

        private void Start()
        {
            objectImage = GetComponent<Image>();
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
        }
    }
}