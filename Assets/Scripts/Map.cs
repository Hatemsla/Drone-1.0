using Drone.Menu;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Drone
{
    public class Map : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TMP_Text mapNameText;
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