using UnityEngine;
using UnityEngine.EventSystems;

namespace Drone
{
    public class UIHint : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject hintPanel;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            hintPanel.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hintPanel.SetActive(false);
        }
    }
}