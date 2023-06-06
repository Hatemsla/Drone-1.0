using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Builder
{
    public class BuilderUI : MonoBehaviour
    {
        public Button floorBtn;
        public Button wallBtn;
        public Button slantBtn;
        public Button gateBtn;
        public Button editorExitBtn;
        public Button gameExitBtn;
        public Button backBtn;
        public Button backEditorTabBtn;
        public Button backGameTabBtn;
        public Button saveBtn;
        public Button testLevelBtn;
        public Button returnToLevelBtn;
        
        public RectTransform pathArrow;
        public Sprite pointerIcon;
        public Sprite outOfScreenIcon;

        public GameObject createPanel;
        public GameObject editButtons;
        public GameObject editorTabPanel;
        public GameObject gameTabPanel;
        public GameObject levelResultPanel;
        public GameObject loadLevelPanel;
        public GameObject uiPanel;
        public GameObject loadPanel;

        public List<Button> createButtons;
    }
}