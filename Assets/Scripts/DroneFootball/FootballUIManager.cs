using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Drone.DroneFootball
{
    public class FootballUIManager : MonoBehaviour
    {
        [Header("DroneView")]
        public GameObject droneView;
        public TMP_Text speedText;
        public TMP_Text timeText;
        public TMP_Text checkpointsCountText;

        [Header("UI")]
        public Button backBtn;
        public Button exitBtn;
        public RectTransform pathArrow;
        public Sprite pointerIcon;
        public Sprite outOfScreenIcon;
        public TMP_Text timeToStartGameText;
        public TMP_Text matchResultText;
        public GameObject descriptionPanel;
        public GameObject matchResultPanel;
        public GameObject backgroundImage;
        public GameObject tabPanel;
        public GameObject uiPanel;
        public GameObject loadPanel;
    }
}