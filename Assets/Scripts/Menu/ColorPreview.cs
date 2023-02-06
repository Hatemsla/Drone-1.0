using System;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class ColorPreview : MonoBehaviour
    {
        public Color color;

        private Image _image;

        private void Start()
        {
            _image = GetComponent<Image>();
        }

        private void Update()
        {
            color = _image.color;
        }
    }
}
