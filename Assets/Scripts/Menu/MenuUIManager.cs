using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class MenuUIManager : MonoBehaviour
    {
        [Header("Auth menu")] 
        public Button authLogBtn;
        public Button authRegBtn;
        public Button authExitBtn;

        [Header("Log menu")] 
        public TMP_InputField logLoginInput;
        public TMP_InputField logPasswordInput;
        public Button logBtn;
        public Button logBackBtn;
        
        [Header("Reg menu")]
        public TMP_InputField regLoginInput;
        public TMP_InputField regPasswordInput;
        public Button regBtn;
        public Button regBackBtn;

        [Header("Start menu")] 
        public Button gameBtn;
        public Button optionsBtn;
        public Button statBtn;
        public Button startExitBtn;
        public Button startExitAccBtn;

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
        public TMP_InputField gameTimeInput;

        [Header("Game menu")] 
        public Toggle difficultToggle;
        public Button raceBtn;
        public Button footballBtn;
        public Button trackBuilderBtn;
        public Button gameBackBtn;
        public Button gameExitBtn;

        [Header("Statistic menu")] 
        public TMP_Text statText1;
        public TMP_Text statText2;
        public Button statBackBtn;

        [Header("Builder menu")] 
        public TMP_InputField levelInput;
        public Button createLevelBtn;
        public Button loadLevelBtn;

        [Header("Menus")]
        public GameObject startMenu;
        public GameObject optionMenu;
        public GameObject gameMenu;
        public List<Menu> menus;
        public List<SubMenu> subMenus;
    }
}
