using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Drone;
using DroneFootball;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager Instance;
        public MenuUIManager menuUIManager;
        public GameData gameData;
        public Resolution[] resolutions;

        public delegate void LevelNameChangedEventHandler(string level);
        public event LevelNameChangedEventHandler LevelNameChanged; 

        private readonly List<string> _difficulties = new List<string>
            {"Супер легко", "Легко", "Нормально", "Сложно", "Невозможно"};

        private readonly List<float> _droneSpeed = new List<float> {0.5f, 0.75f, 1f, 1.5f, 2f};

        private readonly List<float> _gatesSize = new List<float> {3f, 2f, 1.5f, 1.25f, 1f};

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            resolutions = Screen.resolutions.Distinct().ToArray();
            SetDropdownResolutions();
            SetDropdownDifficulties();

            menuUIManager.raceBtn.onClick.AddListener(delegate { StartGame(2); });
            menuUIManager.footballBtn.onClick.AddListener(delegate { StartGame(3); });
            menuUIManager.createLevelBtn.onClick.AddListener(CreateLevel);
            menuUIManager.loadLevelBtn.onClick.AddListener(LoadLevel);
            menuUIManager.playBtn.onClick.AddListener(StartBuilder);
            
            OpenMenu("Start");
            menuUIManager.volumeEffectsSlider.value = gameData.currentEffectsVolume;
            menuUIManager.volumeMusicsSlider.value = gameData.currentMusicsVolume;
            menuUIManager.yawSensitivitySlider.value = gameData.currentYawSensitivity - 1;
            menuUIManager.yawSensitivitySlider.onValueChanged.AddListener(delegate { ChangeYawSensitivity(); });
            menuUIManager.startExitBtn.onClick.AddListener(GameManagerUtils.Exit);
            menuUIManager.isFullscreenToggle.onValueChanged.AddListener(Fullscreen);
            menuUIManager.difficultDropdown.onValueChanged.AddListener(SetDifficult);
            menuUIManager.difficultDropdown.value = gameData.currentDifficultIndex;
            menuUIManager.difficultControlDropdown.onValueChanged.AddListener(SetGameMode);
            menuUIManager.difficultControlDropdown.value = gameData.currentControlDifficultIndex;
            menuUIManager.authExitBtn.onClick.AddListener(GameManagerUtils.Exit);
            menuUIManager.gameBtn.onClick.AddListener(delegate { OpenMenu("Game"); });
            menuUIManager.statBtn.onClick.AddListener(delegate { OpenMenu("Statistics"); });
            menuUIManager.optionsBtn.onClick.AddListener(delegate { OpenMenu("Options"); });
            menuUIManager.gameBackBtn.onClick.AddListener(delegate { OpenMenu("Start"); });
            menuUIManager.optionsBackBtn.onClick.AddListener(delegate { OpenMenu("Start"); });
            menuUIManager.trackBuilderBtn.onClick.AddListener(delegate { OpenMenu("Builder"); });
            menuUIManager.generalSettingsBtn.onClick.AddListener(delegate { OpenSubMenu("GeneralOpt"); });
            menuUIManager.soundSettingsBtn.onClick.AddListener(delegate { OpenSubMenu("SoundOpt"); });
            menuUIManager.controlSettingsBtn.onClick.AddListener(delegate { OpenSubMenu("ControlOpt"); });
            menuUIManager.difficultSettingsBtn.onClick.AddListener(delegate { OpenSubMenu("DifficultOpt"); });
            menuUIManager.customizationSettingsBtn.onClick.AddListener(delegate { OpenSubMenu("CustOpt"); });
            menuUIManager.builderBackBtn.onClick.AddListener(delegate { OpenMenu("Game"); });
            menuUIManager.authLogBtn.onClick.AddListener(delegate { OpenMenu("Log"); });
            menuUIManager.authRegBtn.onClick.AddListener(delegate { OpenMenu("Reg"); });
            menuUIManager.logBackBtn.onClick.AddListener(delegate { OpenMenu("Auth"); });
            menuUIManager.logBackBtn.onClick.AddListener(ClearLogInputs);
            menuUIManager.regBackBtn.onClick.AddListener(delegate { OpenMenu("Auth"); });
            menuUIManager.regBackBtn.onClick.AddListener(ClearRegInputs);
            menuUIManager.startExitAccBtn.onClick.AddListener(delegate { OpenMenu("Auth"); });
            menuUIManager.statBackBtn.onClick.AddListener(delegate { OpenMenu("Start"); });
            menuUIManager.gameTimeInput.text = gameData.gameTimeInSeconds > 0 ? gameData.gameTimeInSeconds.ToString() : "300";

            SetDropdownResolutions();
            menuUIManager.resolutionDropdown.onValueChanged.AddListener(SetResolution);
        }

        private void Update()
        {
            // if (_isMenuScene)
            //     dbManager.UserData.SecondsInGame += Time.deltaTime;
            //
            // if (_isRace)
            //     dbManager.UserStatisticRace.SecondsInGame += Time.deltaTime;
            //
            // if (_isFootball)
            //     dbManager.UserStatisticFootball.SecondsInGame += Time.deltaTime;

            // SetStatistics();
        }

        private void SetDropdownResolutions()
        {
            var options = new List<string>();
            for (var i = 0; i < resolutions.Length; i++)
            {
                options.Add($"{resolutions[i].width} x {resolutions[i].height} {resolutions[i].refreshRateRatio.value}Hz");

                if (resolutions[i].width == Screen.currentResolution.width ||
                    resolutions[i].height == Screen.currentResolution.height)
                    gameData.currentResolutionIndex = i;
            }

            menuUIManager.resolutionDropdown.AddOptions(options);
            menuUIManager.resolutionDropdown.value = gameData.currentResolutionIndex;
            menuUIManager.resolutionDropdown.RefreshShownValue();
        }

        private void SetDropdownDifficulties()
        {
            menuUIManager.difficultDropdown.ClearOptions();
            menuUIManager.difficultDropdown.AddOptions(_difficulties);
            menuUIManager.difficultDropdown.value = 2;
        }

        // public void SetStatistics()
        // {
        //     _statText1 = new StringBuilder();
        //     _statText1.AppendLine("Всего сыграно игр: " + (dbManager.UserStatisticFootball.GamesCount +
        //                                                   dbManager.UserStatisticRace.GamesCount));
        //     _statText1.AppendLine("Игр в дроногонках: " + dbManager.UserStatisticRace.GamesCount);
        //     _statText1.AppendLine("Игр в дронофутболе: " + dbManager.UserStatisticFootball.GamesCount);
        //     _statText1.AppendLine("Всего побед: " + (dbManager.UserStatisticFootball.WinsCount +
        //                                             dbManager.UserStatisticRace.WinsCount));
        //     _statText1.AppendLine("Побед в дроногонках: " + dbManager.UserStatisticFootball.WinsCount);
        //     _statText1.AppendLine("Побед в дронофутболе: " + dbManager.UserStatisticRace.WinsCount);
        //     _statText1.AppendLine("Всего поражений: " + (dbManager.UserStatisticFootball.LosesCount +
        //                                                 dbManager.UserStatisticRace.LosesCount));
        //     _statText1.AppendLine("Поражений в дроногонках: " + dbManager.UserStatisticFootball.LosesCount);
        //     _statText1.AppendLine("Поражений в дронофутболе: " + dbManager.UserStatisticRace.LosesCount);
        //     _statText1.AppendLine("Всего ничьей: " + (dbManager.UserStatisticFootball.GamesCount +
        //         dbManager.UserStatisticRace.GamesCount - (dbManager.UserStatisticFootball.LosesCount +
        //                                                   dbManager.UserStatisticRace.LosesCount +
        //                                                   dbManager.UserStatisticFootball.WinsCount +
        //                                                   dbManager.UserStatisticRace.WinsCount)));
        //     _statText1.AppendLine("Ничьей в дроногонках: " + (dbManager.UserStatisticRace.GamesCount -
        //                                                      (dbManager.UserStatisticRace.LosesCount +
        //                                                       dbManager.UserStatisticRace.WinsCount)));
        //     _statText1.AppendLine("Ничьей в дронофутболе: " + (dbManager.UserStatisticFootball.GamesCount -
        //         dbManager.UserStatisticFootball.LosesCount + dbManager.UserStatisticFootball.WinsCount));
        //     menuUIManager.statText1.text = _statText1.ToString();
        //
        //     _statText2 = new StringBuilder();
        //     _statText2.AppendLine("Всего времени в игре: " + TimeFormat(
        //         dbManager.UserStatisticFootball.SecondsInGame + dbManager.UserStatisticRace.SecondsInGame +
        //         dbManager.UserData.SecondsInGame));
        //     _statText2.AppendLine("Времени в дроногонках: " + TimeFormat(dbManager.UserStatisticRace.SecondsInGame));
        //     _statText2.AppendLine("Времени в дронофутболе: " + TimeFormat(dbManager.UserStatisticFootball.SecondsInGame));
        //     menuUIManager.statText2.text = _statText2.ToString();
        // }

        // private string TimeFormat(float time)
        // {
        //     float hours = Mathf.FloorToInt(time / 3600);
        //     float minutes = Mathf.FloorToInt(time / 60);
        //     float seconds = Mathf.FloorToInt(time % 60);
        //     return $"{hours:00}:{minutes:00}:{seconds:00}";
        // }

        public void SetResolution(int resolutionIndex)
        {
            if (resolutions == null)
                resolutions = Screen.resolutions.Distinct().ToArray();
            var resolution = resolutions[resolutionIndex];
            gameData.currentResolutionIndex = resolutionIndex;
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        public void SetDifficult(int difficultIndex)
        {
            gameData.currentGateScale = _gatesSize[difficultIndex];
            gameData.currentAIDroneSpeed = _droneSpeed[difficultIndex];
            gameData.currentDifficultIndex = difficultIndex;
        }
        
        public void LoadLevel()
        {
            if(!LevelManager.LoadLevel(menuUIManager.levelInput.text))
                return;

            gameData.levelName = menuUIManager.levelInput.text;
            gameData.isLoadLevel = true;
            gameData.isStartBuilder = false;
            SceneManager.LoadScene(4);
        }

        public void CreateLevel()
        {
            if(!LevelManager.IsValidLevelName(menuUIManager.levelInput.text))
                return;
            
            Directory.CreateDirectory(Application.dataPath + "/Levels");
            gameData.levelName = menuUIManager.levelInput.text;
            gameData.isLoadLevel = false;
            gameData.isStartBuilder = false;
            SceneManager.LoadScene(4);
        }

        public void StartBuilder()
        {
            if(!LevelManager.LoadLevel(menuUIManager.levelInput.text))
                return;
            
            gameData.levelName = menuUIManager.levelInput.text;
            gameData.isLoadLevel = false;
            gameData.isStartBuilder = true;
            SceneManager.LoadScene(4);
        }

        public void ClearLogInputs()
        {
            menuUIManager.logLoginInput.text = string.Empty;
            menuUIManager.logPasswordInput.text = string.Empty;
        }

        public void ClearRegInputs()
        {
            menuUIManager.regLoginInput.text = string.Empty;
            menuUIManager.regPasswordInput.text = string.Empty;
        }

        public void OpenSubMenu(string menuName)
        {
            foreach (var menu in menuUIManager.subMenus)
                if (menu.menuName == menuName)
                    menu.Open();
                else
                    menu.Close();
        }

        public void OpenMenu(string menuName)
        {
            foreach (var menu in menuUIManager.menus)
                if (menu.menuName == menuName)
                    menu.Open();
                else
                    menu.Close();
        }

        public void Fullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }

        public void SetGameMode(int value)
        {
            gameData.isSimpleMode = value == 0;
            gameData.currentControlDifficultIndex = value;
        }

        public void ChangeYawSensitivity()
        {
            gameData.currentYawSensitivity = menuUIManager.yawSensitivitySlider.value + 1;
            menuUIManager.yawValue.text = gameData.currentYawSensitivity.ToString("0.0");
        }

        public void OnLevelNameChanged(string level)
        {
            LevelNameChanged?.Invoke(level);
        }

        private void StartGame(int sceneIndex)
        {
            GameTimeHandler();
            OpenMenu("Load");
            GameManager.Instance.asyncLoad.LoadScene(sceneIndex);
        }

        private void GameTimeHandler()
        {
            if (!string.IsNullOrEmpty(menuUIManager.gameTimeInput.text))
                gameData.gameTimeInSeconds = Convert.ToInt32(menuUIManager.gameTimeInput.text);
            else
                gameData.gameTimeInSeconds = 300;
        }
    }
}