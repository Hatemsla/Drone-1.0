using System;
using System.Collections.Generic;
using System.Linq;
using DB;
using DroneFootball;
using DroneRace;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu
{
    public class MenuManager : MonoBehaviour
    {
        public bool isSimpleMode;
        public int currentDifficultIndex;
        public int currentResolutionIndex = 0;
        public float currentYawSensitivity = 1;
        public float currentVolume;
        public DBManager dbManager;
        public MenuUIManager menuUIManager;
        public RaceController raceController;
        public FootballController footballController;
        public ColorPreview botsColorPreview;
        public ColorPreview playerColorPreview;
        public Resolution[] Resolutions;

        private readonly List<string> _difficulties = new List<string>
            {"Супер легко", "Легко", "Нормально", "Сложно", "Невозможно"};

        private readonly List<float> _gatesSize = new List<float> {3f, 2f, 1.5f, 1.25f, 1f};
        private readonly List<float> _droneSpeed = new List<float> {0.5f, 0.75f, 1f, 1.5f, 2f};
        private float _currentGateScale;
        private float _currentAIDroneSpeed;
        private bool _isFootball;
        private bool _isRace;
        private bool _isMenuScene;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            dbManager = GetComponent<DBManager>();
            Resolutions = Screen.resolutions.Distinct().ToArray();
            SetDropdownResolutions();
            SetDropdownDifficulties();

            menuUIManager.raceBtn.onClick.AddListener(delegate { StartGame(1); });
            menuUIManager.footballBtn.onClick.AddListener(delegate { StartGame(2); });
            menuUIManager.difficultToggle.isOn = false;
            menuUIManager.volumeSlider.value = 1;
            botsColorPreview = menuUIManager.botColorPicker.GetComponentInChildren<ColorPreview>();
            playerColorPreview = menuUIManager.playerColorPicker.GetComponentInChildren<ColorPreview>();
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Update()
        {
            if(_isMenuScene)
                dbManager.UserData.SecondsInGame += Time.deltaTime;

            if (_isRace)
                dbManager.UserStatisticRace.SecondsInGame += Time.deltaTime;
        }

        private void SetDropdownResolutions()
        {
            var options = new List<string>();
            for (int i = 0; i < Resolutions.Length; i++)
            {
                options.Add($"{Resolutions[i].width} x {Resolutions[i].height} {Resolutions[i].refreshRate}Hz");

                if (Resolutions[i].width == Screen.currentResolution.width ||
                    Resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }

            menuUIManager.resolutionDropdown.AddOptions(options);
            menuUIManager.resolutionDropdown.value = currentResolutionIndex;
            menuUIManager.resolutionDropdown.RefreshShownValue();
        }

        private void SetDropdownDifficulties()
        {
            menuUIManager.difficultDropdown.ClearOptions();
            menuUIManager.difficultDropdown.AddOptions(_difficulties);
            menuUIManager.difficultDropdown.value = 2;
        }

        public void SetResolution(int resolutionIndex)
        {
            if (Resolutions == null)
                Resolutions = Screen.resolutions.Distinct().ToArray();
            Resolution resolution = Resolutions[resolutionIndex];
            currentResolutionIndex = resolutionIndex;
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        public void SetDifficult(int difficultIndex)
        {
            _currentGateScale = _gatesSize[difficultIndex];
            _currentAIDroneSpeed = _droneSpeed[difficultIndex];
            currentDifficultIndex = difficultIndex;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            AudioListener.volume = currentVolume;
            if (scene.buildIndex == 0)
            {
                _isMenuScene = true;
                _isRace = false;
                _isFootball = false;
                var dontDestroyMenuManager = FindObjectsOfType<MenuManager>();
                var dontDestroyDbManager = FindObjectsOfType<DBManager>();
                foreach (var obj in dontDestroyMenuManager)
                    if (obj.transform.gameObject != transform.gameObject)
                        Destroy(obj);

                foreach (var obj in dontDestroyDbManager)
                    if (obj.transform.gameObject != transform.gameObject)
                        Destroy(obj);

                menuUIManager = FindObjectOfType<MenuUIManager>();
                menuUIManager.volumeSlider.value = currentVolume;
                menuUIManager.volumeSlider.onValueChanged.AddListener(delegate { ChangeVolume(); });
                menuUIManager.yawSensitivitySlider.value = currentYawSensitivity - 1;
                menuUIManager.yawSensitivitySlider.onValueChanged.AddListener(delegate { ChangeYawSensitivity(); });
                menuUIManager.startExitBtn.onClick.AddListener(Exit);
                menuUIManager.optionsExitBtn.onClick.AddListener(Exit);
                menuUIManager.gameExitBtn.onClick.AddListener(Exit);
                menuUIManager.isFullscreenToggle.onValueChanged.AddListener(Fullscreen);
                menuUIManager.difficultDropdown.onValueChanged.AddListener(SetDifficult);
                menuUIManager.difficultDropdown.value = currentDifficultIndex;
                menuUIManager.difficultToggle.onValueChanged.AddListener(SetGameMode);
                menuUIManager.difficultToggle.isOn = false;
                menuUIManager.raceBtn.onClick.AddListener(delegate { StartGame(1); });
                menuUIManager.footballBtn.onClick.AddListener(delegate { StartGame(2); });
                botsColorPreview = menuUIManager.botColorPicker.GetComponentInChildren<ColorPreview>();
                playerColorPreview = menuUIManager.playerColorPicker.GetComponentInChildren<ColorPreview>();
                menuUIManager.authExitBtn.onClick.AddListener(Exit);
                menuUIManager.gameBtn.onClick.AddListener(delegate { OpenMenu("Game"); });
                menuUIManager.optionsBtn.onClick.AddListener(delegate { OpenMenu("Options"); });
                menuUIManager.gameBackBtn.onClick.AddListener(delegate { OpenMenu("Start"); });
                menuUIManager.optionsBackBtn.onClick.AddListener(delegate { OpenMenu("Start"); });
                menuUIManager.generalSettingsBtn.onClick.AddListener(delegate { OpenSubMenu("GeneralOpt"); });
                menuUIManager.soundSettingsBtn.onClick.AddListener(delegate { OpenSubMenu("SoundOpt"); });
                menuUIManager.controlSettingsBtn.onClick.AddListener(delegate { OpenSubMenu("ControlOpt"); });
                menuUIManager.customizationSettingsBtn.onClick.AddListener(delegate { OpenSubMenu("CustOpt"); });
                menuUIManager.authLogBtn.onClick.AddListener(delegate { OpenMenu("Log"); });
                menuUIManager.authRegBtn.onClick.AddListener(delegate { OpenMenu("Reg"); });
                menuUIManager.logBackBtn.onClick.AddListener(delegate { OpenMenu("Auth"); });
                menuUIManager.logBackBtn.onClick.AddListener(ClearLogInputs);
                menuUIManager.logBtn.onClick.AddListener(delegate { dbManager.Login(); });
                menuUIManager.regBtn.onClick.AddListener(delegate { dbManager.Registration(); });
                menuUIManager.regBackBtn.onClick.AddListener(delegate { OpenMenu("Auth"); });
                menuUIManager.regBackBtn.onClick.AddListener(ClearRegInputs);
                menuUIManager.startExitAccBtn.onClick.AddListener(delegate { OpenMenu("Auth"); });
                menuUIManager.startExitAccBtn.onClick.AddListener(dbManager.SaveUserResolution);
                menuUIManager.startExitAccBtn.onClick.AddListener(dbManager.SaveUserSettings);
                menuUIManager.startExitAccBtn.onClick.AddListener(dbManager.SaveUserData);

                SetDropdownResolutions();
                menuUIManager.resolutionDropdown.onValueChanged.AddListener(SetResolution);
            }
            else if (scene.buildIndex == 1)
            {
                _isMenuScene = false;
                _isRace = true;
                _isFootball = false;
                raceController = FindObjectOfType<RaceController>();
                raceController.currentAIDroneSpeed = _currentAIDroneSpeed;
                raceController.raceUIManager.backBtn.onClick.AddListener(BackToMenu);
                raceController.raceUIManager.exitBtn.onClick.AddListener(Exit);
                raceController.isSimpleMode = isSimpleMode;
                raceController.droneRaceController.yawPower = currentYawSensitivity;
                raceController.droneRaceController.droneMeshRenderer.material.SetColor("_Color", playerColorPreview.color);
                raceController.droneRaceController.droneMeshRenderer.material.SetColor("_EmissionColor", playerColorPreview.color);
                raceController.droneRaceAI.droneMeshRenderer.material.SetColor("_Color", botsColorPreview.color);
                raceController.droneRaceAI.droneMeshRenderer.material.SetColor("_EmissionColor", botsColorPreview.color);
            }
            else if (scene.buildIndex == 2)
            {
                _isMenuScene = false;
                _isRace = false;
                _isFootball = true;
                footballController = FindObjectOfType<FootballController>();
                footballController.currentGateScale = _currentGateScale;
                footballController.currentAIDroneSpeed = _currentAIDroneSpeed;
                footballController.footballUIManager.backBtn.onClick.AddListener(BackToMenu);
                footballController.footballUIManager.exitBtn.onClick.AddListener(Exit);
                footballController.isSimpleMode = isSimpleMode;
                footballController.droneFootballController.yawPower = currentYawSensitivity;
                footballController.droneFootballController.droneMeshRenderer.material.SetColor("_Color", playerColorPreview.color);
                footballController.droneFootballController.droneMeshRenderer.material.SetColor("_EmissionColor", playerColorPreview.color);
                footballController.droneFootballAIList[0].droneMeshRenderer.material.SetColor("_Color", botsColorPreview.color);
                footballController.droneFootballAIList[0].droneMeshRenderer.material.SetColor("_EmissionColor", botsColorPreview.color);
                footballController.droneFootballAIList[1].droneMeshRenderer.material.SetColor("_Color", botsColorPreview.color);
                footballController.droneFootballAIList[1].droneMeshRenderer.material.SetColor("_EmissionColor", botsColorPreview.color);
            }
        }

        public void ClearLogInputs()
        {
            menuUIManager.logLoginInput.text = String.Empty;
            menuUIManager.logPasswordInput.text = String.Empty;
        }

        public void ClearRegInputs()
        {
            menuUIManager.regLoginInput.text = String.Empty;
            menuUIManager.regPasswordInput.text = String.Empty;
        }

        
        public void OpenSubMenu(string menuName)
        {
            foreach (var menu in menuUIManager.subMenus)
            {
                if (menu.menuName == menuName)
                {
                    menu.Open();
                }
                else
                {
                    menu.Close();
                }
            }   
        }

        public void OpenMenu(string menuName)
        {
            foreach (var menu in menuUIManager.menus)
            {
                if (menu.menuName == menuName)
                {
                    menu.Open();
                }
                else
                {
                    menu.Close();
                }
            }            
        }

        public void BackToMenu()
        {
            SceneManager.LoadScene(0);
        }

        public void Exit()
        {
            dbManager.SaveUserResolution();
            dbManager.SaveUserSettings();
            dbManager.SaveUserData();
            Application.Quit();
        }

        public void Fullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }

        public void SetGameMode(bool mode)
        {
            isSimpleMode = !mode;
        }

        public void ChangeVolume()
        {
            AudioListener.volume = menuUIManager.volumeSlider.value;
            currentVolume = menuUIManager.volumeSlider.value;
        }

        public void ChangeYawSensitivity()
        {
            currentYawSensitivity = menuUIManager.yawSensitivitySlider.value + 1;
        }

        private void StartGame(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }
}
