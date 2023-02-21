using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Menu
{
    [Serializable]
    public class ColorEvent : UnityEvent<Color> { }
    
    public class ColorPicker : MonoBehaviour
    {
        public RectTransform rect;
        public Color color;
        public ColorEvent onColorSelect;

        private void Start()
        {
            rect = GetComponent<RectTransform>();
            color = GetComponent<Image>().color;
        }

        private void Update()
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    onColorSelect?.Invoke(color);
                }
            }
        }
    }
}