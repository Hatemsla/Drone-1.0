using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DroneRace
{
    public class DroneRaceUIManager : MonoBehaviour
    {
        public Button backBtn;
        public Button exitBtn;
        public RectTransform pathArrow;
        public Sprite pointerIcon;
        public Sprite outOfScreenIcon;
        public TMP_Text timeToStartGameText;
        public TMP_Text timeToEndGameText;
        public TMP_Text matchResultText;
        public TMP_Text racePositionText;
        public GameObject descriptionPanel;
        public GameObject matchResultPanel;
        public GameObject backgroundImage;
        public GameObject tabPanel;
    }
}
