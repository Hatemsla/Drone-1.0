using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetPointer : MonoBehaviour
{
    public Transform target; // цель
    public RectTransform pointerUI; // объект Image UI
    public Sprite pointerIcon; // иконка когда цель в поле видимости
    public Sprite outOfScreenIcon; // иконка когда цель за приделами экрана	
    public float interfaceScale = 100; // масштаб интерфейса

    private Vector3 _startPointerSize;
    private Camera _mainCamera;


    private void Awake()
    {
        _startPointerSize = pointerUI.sizeDelta;
        _mainCamera = Camera.main;
    }
    private void LateUpdate()
    {
        Vector3 realPos = _mainCamera.WorldToScreenPoint(target.position); // получениее экранных координат объекта
        Rect rect = new Rect(0, 0, Screen.width, Screen.height);

        Vector3 outPos = realPos;
        float direction = 1;

        pointerUI.GetComponent<Image>().sprite = outOfScreenIcon;

        if (!IsBehind(target.position)) // если цель спереди
        {
            if (rect.Contains(realPos)) // и если цель в окне экрана
            {
                pointerUI.GetComponent<Image>().sprite = pointerIcon;
            }
        }
        else // если цель cзади
        {
            realPos = -realPos;
            outPos = new Vector3(realPos.x, 0, 0); // позиция иконки - снизу
            if (_mainCamera.transform.position.y < target.position.y)
            {
                direction = -1;
                outPos.y = Screen.height; // позиция иконки - сверху				
            }
        }
        // ограничиваем позицию областью экрана
        float offset = pointerUI.sizeDelta.x / 2;
        outPos.x = Mathf.Clamp(outPos.x, offset, Screen.width - offset);
        outPos.y = Mathf.Clamp(outPos.y, offset, Screen.height - offset);

        Vector3 pos = realPos - outPos; // направление к цели из PointerUI 

        RotatePointer(direction * pos);

        pointerUI.sizeDelta = new Vector2(_startPointerSize.x / 100 * interfaceScale, _startPointerSize.y / 100 * interfaceScale);
        pointerUI.anchoredPosition = outPos;
    }
    private bool IsBehind(Vector3 point) // true если point сзади камеры
    {
        Vector3 forward = _mainCamera.transform.TransformDirection(Vector3.forward);
        Vector3 toOther = point - _mainCamera.transform.position;
        if (Vector3.Dot(forward, toOther) < 0) return true;
        return false;
    }
    private void RotatePointer(Vector2 direction) // поворачивает PointerUI в направление direction
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        pointerUI.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
