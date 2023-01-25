using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Menu
{
    public class ScrollRectAutoScroll : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public float scrollSpeed = 10f;

        private Vector2 _mNextScrollPosition = Vector2.up;
        private ScrollRect _mScrollRect;
        private readonly List<Selectable> _mSelectables = new List<Selectable>();
        private bool _mouseOver;

        private void Awake()
        {
            _mScrollRect = GetComponent<ScrollRect>();
        }

        private void Start()
        {
            if (_mScrollRect) _mScrollRect.content.GetComponentsInChildren(_mSelectables);
            ScrollToSelected(true);
        }

        private void Update()
        {
            // Scroll via input.
            InputScroll();
            if (!_mouseOver)
                // Lerp scrolling code.
                _mScrollRect.normalizedPosition = Vector2.Lerp(_mScrollRect.normalizedPosition, _mNextScrollPosition,
                    scrollSpeed * Time.deltaTime);
            else
                _mNextScrollPosition = _mScrollRect.normalizedPosition;
        }

        private void OnEnable()
        {
            if (_mScrollRect) _mScrollRect.content.GetComponentsInChildren(_mSelectables);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _mouseOver = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _mouseOver = false;
            ScrollToSelected(false);
        }

        private void InputScroll()
        {
            if (_mSelectables.Count > 0)
                if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical") ||
                    Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
                    ScrollToSelected(false);
        }

        private void ScrollToSelected(bool quickScroll)
        {
            var selectedIndex = -1;
            var selectedElement = EventSystem.current.currentSelectedGameObject
                ? EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>()
                : null;

            if (selectedElement) selectedIndex = _mSelectables.IndexOf(selectedElement);
            if (selectedIndex > -1)
            {
                if (quickScroll)
                {
                    _mScrollRect.normalizedPosition =
                        new Vector2(0, 1 - selectedIndex / ((float) _mSelectables.Count - 1));
                    _mNextScrollPosition = _mScrollRect.normalizedPosition;
                }
                else
                {
                    _mNextScrollPosition = new Vector2(0, 1 - selectedIndex / ((float) _mSelectables.Count - 1));
                }
            }
        }
    }
}