using Menu;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DroneFootball
{
    public class Map : MonoBehaviour, IPointerClickHandler
    {
        public TMP_Text mapNameText;
        public string mapName;

        private void Start()
        {
            mapNameText.text = mapName;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            MenuManager.Instance.menuUIManager.levelInput.text = mapName;
        }
    }
}