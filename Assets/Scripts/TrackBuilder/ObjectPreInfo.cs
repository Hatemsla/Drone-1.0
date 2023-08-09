using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Builder
{
    public sealed class ObjectPreInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private RectTransform preInfo;
        [SerializeField] private TMP_Text objectNameText;
        [SerializeField] private TMP_Text objectDescText;
        [SerializeField] private Image objectPreImage;
        [SerializeField] private Image preObjectImage;
        [SerializeField] private Image objectImage;
        [SerializeField] private Image preInfoBack;
        [SerializeField] private Image preInfoEllipseImage;
        [SerializeField] private Image background;
        [SerializeField] private Sprite backgroundDefault;
        [SerializeField] private Sprite backgroundHover;
        [SerializeField] private Sprite preInfoBackground;
        [SerializeField] private Sprite preInfoEllipse;
        [SerializeField] private Color objectNameColor;
        public string objectName;
        public string objectDesc;

        private void Start()
        {
            background = GetComponent<Image>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            preInfo.gameObject.SetActive(true);
            objectNameText.text = objectName;
            objectNameText.color = objectNameColor;
            objectDescText.text = objectDesc;
            background.sprite = backgroundHover;
            preInfoBack.sprite = preInfoBackground;
            preInfoEllipseImage.sprite = preInfoEllipse;
            preObjectImage.sprite = objectImage.sprite;

            Vector2 newPosition = preInfo.localPosition;
            newPosition.x = transform.localPosition.x;
            preInfo.localPosition = newPosition;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            preInfo.gameObject.SetActive(false);
            background.sprite = backgroundDefault;
        }
    }
}