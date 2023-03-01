using System;
using Unity.VisualScripting;
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
        private bool _isPickerOpen;

        private void Start()
        {
            colorPicker.SetActive(false);
            image = GetComponent<Image>();
            _colorPreviewBtn = GetComponent<Button>();
            _colorPreviewBtn.onClick.AddListener(OpenColorPicker);
        }

        private void Update()
        {
            color = image.color;
        }

        private void OpenColorPicker()
        {
            _isPickerOpen = !_isPickerOpen;
            colorPicker.SetActive(_isPickerOpen);
        }

        public void CloseColorPicker()
        {
            _isPickerOpen = false;
            colorPicker.SetActive(_isPickerOpen);
        }
    }
}
