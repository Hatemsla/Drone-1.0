using System;
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
        public Texture2D texture;
        public Color color;
        public ColorEvent onColorSelect;

        private void Start()
        {
            rect = GetComponent<RectTransform>();
            texture = GetComponent<Image>().mainTexture as Texture2D;
        }

        private void Update()
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition))
            {
                Vector2 delta;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, Input.mousePosition,
                    null, out delta);

                float width = rect.rect.width;
                float height = rect.rect.height;
                delta += new Vector2(width * 0.5f, height * 0.5f);

                float x = Mathf.Clamp(delta.x / width, 0f, 1f);
                float y = Mathf.Clamp(delta.y / height, 0f, 1f);

                int texX = Mathf.RoundToInt(x * texture.width);
                int texY = Mathf.RoundToInt(y * texture.height);

                color = texture.GetPixel(texX, texY);

                if (Input.GetMouseButtonDown(0))
                {
                    onColorSelect?.Invoke(color);
                }
            }
        }
    }
}