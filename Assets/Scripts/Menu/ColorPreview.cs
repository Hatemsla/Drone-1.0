using System;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class ColorPreview : MonoBehaviour
    {
        public Color color;
        public Image image;
        public GameObject colorPicker;

        private Button _colorPreviewBtn;
        private bool _isPickerOpen = true;

        private void Start()
        {
            colorPicker.SetActive(false);
            image = GetComponent<Image>();
            _colorPreviewBtn = GetComponent<Button>();
            _colorPreviewBtn.onClick.AddListener(ColorPickerOpen);
        }

        private void Update()
        {
            color = image.color;
        }

        private void ColorPickerOpen()
        {
            colorPicker.SetActive(_isPickerOpen);
            _isPickerOpen = !_isPickerOpen;
        }
    }
}
