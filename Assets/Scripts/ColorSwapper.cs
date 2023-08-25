using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Drone
{
    public class ColorSwapper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private Color defaultColor;
        [SerializeField] private Color hoverColor;

        private void Start()
        {
            if(text == null)
                text = GetComponent<TMP_Text>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            text.color = hoverColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            text.color = defaultColor;
        }
    }
}