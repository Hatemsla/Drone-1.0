using System;
using System.Collections.Generic;
using System.Linq;
using Drone;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Drone.Builder
{
    public class BuilderUI : MonoBehaviour
    {
        [Header("Drone view")]
        public GameObject droneView;
        public TMP_Text speedText;
        public TMP_Text batteryText;
        public TMP_Text timeText;
        public TMP_Text terminalTimeText;
        public TMP_Text checkpointsCountText;
        public TMP_Text coinsCountText;
        public TMP_Text crystalsCountText;
        public TMP_Text restoreHealthText;
        public Prompt prompt;
        public HelpMessage helpMessage;
        public DroneBars healthBar;
        public DroneBars armorBar;
        
        [Header("UI")]
        public Button floorBtn;
        public Button wallBtn;
        public Button slantBtn;
        public Button gateBtn;
        public Button editorGameExitBtn;
        public Button editorExitBtn;
        public Button gameExitBtn;
        public Button backEditorTabBtn;
        public Button backGameTabBtn;
        public Button backBuilderTabBtn;
        public Button saveBtn;
        public Button testLevelBtn;
        public Button returnToLevelBtn;
        
        public RectTransform pathArrow;
        public Image pathArrowImage;
        public Sprite pointerIcon;
        public Sprite outOfScreenIcon;

        public GameObject exitBuilderPanel;
        public GameObject createPanel;
        public GameObject editButtons;
        public GameObject editorTabPanel;
        public GameObject gameTabPanel;
        public GameObject levelResultPanel;
        public GameObject loadLevelPanel;
        public GameObject uiPanel;
        public GameObject portUI;
        public GameObject passwordUI;
        public GameObject terminalUI;
        public GameObject loadPanel;
        public GameObject objectEditPanel;
        public GameObject saveLevelPanel;
        public GameObject createButtonsParent;
        public GameObject securityCameraView;

        public List<Button> createButtons;
        public List<ObjectPreInfo> objectPreInfos;
        
        [Header("ExitTabPanel")]
        public GameObject exitTabPanel;
        public Button confirmExitBtn;
        public Button cancelExitBtn;
        public Slider targetSpeedSlider1;
        public Slider targetSpeedSlider2;
        public Slider targetSpeedSlider3;
        public TMP_Text targetText1Value;
        public TMP_Text targetText2Value;
        public TMP_Text targetText3Value;
        
        [Header("Animations")]
        public Animator savePanelAnimator;

        private void Awake()
        {
            foreach (var createButton in createButtons)
            {
                objectPreInfos.Add(createButton.GetComponent<ObjectPreInfo>());
            }
        }

        private void Start()
        {
            LevelManager.StartLevelLoading += ActivateLoadUI;
            BuilderManager.Instance.LoadingCompleteEvent += ActivateBuilderUI;
            BuilderManager.Instance.StartGame += ActivateDroneUI;
            BuilderManager.Instance.StopGame += ActivateBuilderUI;
        }

        private void OnDestroy()
        {
            LevelManager.StartLevelLoading -= ActivateLoadUI;
            BuilderManager.Instance.LoadingCompleteEvent -= ActivateBuilderUI;
            BuilderManager.Instance.StartGame -= ActivateDroneUI;
            BuilderManager.Instance.StopGame -= ActivateBuilderUI;
        }

        private void ActivateDroneUI()
        {
            droneView.SetActive(true);
            exitBuilderPanel.SetActive(false);
            createPanel.SetActive(false);
            editButtons.SetActive(false);
        }
        
        private void ActivateBuilderUI()
        {
            createPanel.SetActive(true);
            createPanel.SetActive(true);
            droneView.SetActive(false);
            pathArrow.gameObject.SetActive(false);
            editorTabPanel.SetActive(false);
            gameTabPanel.SetActive(false);
            loadLevelPanel.SetActive(false);
        }

        private void ActivateLoadUI()
        {
            pathArrow.gameObject.SetActive(false);
            editButtons.SetActive(false);
            createPanel.SetActive(false);
            loadLevelPanel.SetActive(true);
        }
    }
}