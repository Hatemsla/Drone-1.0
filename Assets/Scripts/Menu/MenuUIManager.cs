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
        public TMP_Dropdown difficultControlDropdown;
        public Button optionsBackBtn;
        public Button generalSettingsBtn;
        public Button soundSettingsBtn;
        public Button controlSettingsBtn;
        public Button difficultSettingsBtn;
        public Button customizationSettingsBtn;
        public Slider volumeEffectsSlider;
        public Slider volumeMusicsSlider;
        public Slider yawSensitivitySlider;
        public GameObject generalSettings;
        public GameObject soundSettings;
        public GameObject controlSettings;
        public GameObject difficultSettings;
        public GameObject customizationSettings;
        public TMP_InputField gameTimeInput;
        public TMP_Text effectsVolumeValue;
        public TMP_Text musicsVolumeValue;
        public TMP_Text yawValue;

        [Header("Game menu")] 
        public Button raceBtn;
        public Button footballBtn;
        public Button trackBuilderBtn;
        public Button gameBackBtn;

        [Header("Statistic menu")] 
        public TMP_Text statText1;
        public TMP_Text statText2;
        public Button statBackBtn;

        [Header("Builder menu")] 
        public TMP_InputField levelInput;
        public Button createLevelBtn;
        public Button loadLevelBtn;
        public Button playBtn;
        public Button builderBackBtn;

        [Header("Menus")]
        public GameObject startMenu;
        public GameObject optionMenu;
        public GameObject gameMenu;
        public List<Menu> menus;
        public List<SubMenu> subMenus;
    }
}
