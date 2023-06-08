using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


public class ColorSwapper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text text;
    public Color defaultColor;
    public Color hoverColor;

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