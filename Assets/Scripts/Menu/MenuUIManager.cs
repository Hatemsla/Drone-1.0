using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class MenuUIManager : MonoBehaviour
    {
        [Header("Start menu")] 
        public Button gameBtn;
        public Button optionsBtn;
        public Button startExitBtn;
        
        [Header("Options menu")]
        public Toggle isFullscreenToggle;
        public TMP_Dropdown resolutionDropdown;
        public TMP_Dropdown difficultDropdown;
        public Button optionsBackBtn;
        public Button optionsExitBtn;
        public Button generalSettingsBtn;
        public Button soundSettingsBtn;
        public Button controlSettingsBtn;
        public Button customizationSettingsBtn;
        public Slider volumeSlider;
        public Slider yawSensitivitySlider;
        public GameObject generalSettings;
        public GameObject soundSettings;
        public GameObject controlSettings;
        public GameObject customizationSettings;
        public GameObject botColorPicker;
        public GameObject playerColorPicker;
        public GameObject botColorImage;
        public GameObject playerColorImage;

        [Header("Game menu")] 
        public Toggle difficultToggle;
        public Button raceBtn;
        public Button footballBtn;
        public Button gameBackBtn;
        public Button gameExitBtn;
        
        [Header("Menus")]
        public GameObject startMenu;
        public GameObject optionMenu;
        public GameObject gameMenu;
    }
}
