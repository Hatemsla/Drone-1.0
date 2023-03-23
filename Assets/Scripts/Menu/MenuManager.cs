using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DB;
using DroneFootball;
using DroneRace;
using Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu
{
    public class MenuManager : MonoBehaviour
    {
        public bool isSimpleMode;
        public int currentDifficultIndex;
        public int currentResolutionIndex;
        public float currentYawSensitivity = 1;
        public float currentVolume;
        public DBManager dbManager;
        public Server server;
        public MenuUIManager menuUIManager;
        public RaceController raceController;
        public FootballController footballController;
        public ColorPreview botsColorPreview;
        public ColorPreview playerColorPreview;
        public Resolution[] Resolutions;

        private readonly List<string> _difficulties = new List<string>
            {"Супер легко", "Легко", "Нормально", "Сложно", "Невозможно"};

        private readonly List<float> _droneSpeed = new List<float> {0.5f, 0.75f, 1f, 1.5f, 2f};

        private readonly List<float> _gatesSize = new List<float> {3f, 2f, 1.5f, 1.25f, 1f};
        private float _currentAIDroneSpeed;
        private float _currentGateScale;
        private int _gameTimeInSeconds;
        private bool _isFootball;
        private bool _isMenuScene = true;
        private bool _isRace;
        private StringBuilder _statText1;
        private StringBuilder _statText2;
        private Color _botsColorPreview;
        private Color _playerColorPreview;
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            dbManager = GetComponent<DBManager>();
            server = GetComponent<Server>();
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
            if (_isMenuScene)
                dbManager.UserData.SecondsInGame += Time.deltaTime;

            if (_isRace)
                dbManager.UserStatisticRace.SecondsInGame += Time.deltaTime;

            if (_isFootball)
                dbManager.UserStatisticFootball.SecondsInGame += Time.deltaTime;

            SetStatistics();
            _botsColorPreview = botsColorPreview.color;
            _playerColorPreview = playerColorPreview.color;
        }

        private void SetDropdownResolutions()
        {
            var options = new List<string>();
            for (var i = 0; i < Resolutions.Length; i++)
            {
                options.Add($"{Resolutions[i].width} x {Resolutions[i].height} {Resolutions[i].refreshRate}Hz");

                if (Resolutions[i].width == Screen.currentResolution.width ||
                    Resolutions[i].height == Screen.currentResolution.height)
                    currentResolutionIndex = i;
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

        public void SetStatistics()
        {
            _statText1 = new StringBuilder();
            _statText1.AppendLine("Всего сыграно игр: " + (dbManager.UserStatisticFootball.GamesCount +
                                                          dbManager.UserStatisticRace.GamesCount));
            _statText1.AppendLine("Игр в дроногонках: " + dbManager.UserStatisticRace.GamesCount);
            _statText1.AppendLine("Игр в дронофутболе: " + dbManager.UserStatisticFootball.GamesCount);
            _statText1.AppendLine("Всего побед: " + (dbManager.UserStatisticFootball.WinsCount +
                                                    dbManager.UserStatisticRace.WinsCount));
            _statText1.AppendLine("Побед в дроногонках: " + dbManager.UserStatisticFootball.WinsCount);
            _statText1.AppendLine("Побед в дронофутболе: " + dbManager.UserStatisticRace.WinsCount);
            _statText1.AppendLine("Всего поражений: " + (dbManager.UserStatisticFootball.LosesCount +
                                                        dbManager.UserStatisticRace.LosesCount));
            _statText1.AppendLine("Поражений в дроногонках: " + dbManager.UserStatisticFootball.LosesCount);
            _statText1.AppendLine("Поражений в дронофутболе: " + dbManager.UserStatisticRace.LosesCount);
            _statText1.AppendLine("Всего ничьей: " + (dbManager.UserStatisticFootball.GamesCount +
                dbManager.UserStatisticRace.GamesCount - (dbManager.UserStatisticFootball.LosesCount +
                                                          dbManager.UserStatisticRace.LosesCount +
                                                          dbManager.UserStatisticFootball.WinsCount +
                                                          dbManager.UserStatisticRace.WinsCount)));
            _statText1.AppendLine("Ничьей в дроногонках: " + (dbManager.UserStatisticRace.GamesCount -
                                                             (dbManager.UserStatisticRace.LosesCount +
                                                              dbManager.UserStatisticRace.WinsCount)));
            _statText1.AppendLine("Ничьей в дронофутболе: " + (dbManager.UserStatisticFootball.GamesCount -
                dbManager.UserStatisticFootball.LosesCount + dbManager.UserStatisticFootball.WinsCount));
            menuUIManager.statText1.text = _statText1.ToString();

            _statText2 = new StringBuilder();
            _statText2.AppendLine("Всего времени в игре: " + TimeFormat(
                dbManager.UserStatisticFootball.SecondsInGame + dbManager.UserStatisticRace.SecondsInGame +
                dbManager.UserData.SecondsInGame));
            _statText2.AppendLine("Времени в дроногонках: " + TimeFormat(dbManager.UserStatisticRace.SecondsInGame));
            _statText2.AppendLine("Времени в дронофутболе: " + TimeFormat(dbManager.UserStatisticFootball.SecondsInGame));
            menuUIManager.statText2.text = _statText2.ToString();
        }

        private string TimeFormat(float time)
        {
            float hours = Mathf.FloorToInt(time / 3600);
            float minutes = Mathf.FloorToInt(time / 60);
            float seconds = Mathf.FloorToInt(time % 60);
            return $"{hours:00}:{minutes:00}:{seconds:00}";
        }

        public void SetResolution(int resolutionIndex)
        {
            if (Resolutions == null)
                Resolutions = Screen.resolutions.Distinct().ToArray();
            var resolution = Resolutions[resolutionIndex];
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
                var dontDestroyServer = FindObjectsOfType<Server>();
                foreach (var obj in dontDestroyMenuManager)
                    if (obj.transform.gameObject != transform.gameObject)
                        Destroy(obj);

                foreach (var obj in dontDestroyDbManager)
                    if (obj.transform.gameObject != transform.gameObject)
                        Destroy(obj);

                foreach (var obj in dontDestroyServer)
                    if (obj.transform.gameObject != transform.gameObject)
                        Destroy(obj);

                menuUIManager = FindObjectOfType<MenuUIManager>();
                OpenMenu("Start");
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
                botsColorPreview.GetComponent<Image>().color = _botsColorPreview;
                playerColorPreview.GetComponent<Image>().color = _playerColorPreview;
                menuUIManager.authExitBtn.onClick.AddListener(Exit);
                menuUIManager.gameBtn.onClick.AddListener(delegate { OpenMenu("Game"); });
                menuUIManager.statBtn.onClick.AddListener(delegate { OpenMenu("Statistics"); });
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
                // menuUIManager.startExitAccBtn.onClick.AddListener(dbManager.SaveUserFootballStatistic);
                // menuUIManager.startExitAccBtn.onClick.AddListener(dbManager.SaveUserRaceStatistic);
                // menuUIManager.startExitAccBtn.onClick.AddListener(dbManager.SaveUserResolution);
                // menuUIManager.startExitAccBtn.onClick.AddListener(dbManager.SaveUserSettings);
                // menuUIManager.startExitAccBtn.onClick.AddListener(dbManager.SaveUserData);
                menuUIManager.statBackBtn.onClick.AddListener(delegate { OpenMenu("Start"); });
                menuUIManager.gameTimeInput.text = _gameTimeInSeconds.ToString();

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
                raceController.droneRaceController.droneMeshRenderer.material.SetColor("_Color",
                    _playerColorPreview);
                raceController.droneRaceController.droneMeshRenderer.material.SetColor("_EmissionColor",
                    _playerColorPreview);
                raceController.droneRaceAI.droneMeshRenderer.material.SetColor("_Color", _botsColorPreview);
                raceController.droneRaceAI.droneMeshRenderer.material.SetColor("_EmissionColor",
                    _botsColorPreview);
                server.droneRaceController = raceController.droneRaceController;
                
                raceController.timer.timeForEndGame = _gameTimeInSeconds;
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
                footballController.droneFootballController.droneMeshRenderer.material.SetColor("_Color",
                    _playerColorPreview);
                footballController.droneFootballController.droneMeshRenderer.material.SetColor("_EmissionColor",
                    _playerColorPreview);
                footballController.droneFootballAIList[0].droneMeshRenderer.material
                    .SetColor("_Color", _botsColorPreview);
                footballController.droneFootballAIList[0].droneMeshRenderer.material
                    .SetColor(EmissionColor, _botsColorPreview);
                footballController.droneFootballAIList[1].droneMeshRenderer.material
                    .SetColor("_Color", _botsColorPreview);
                footballController.droneFootballAIList[1].droneMeshRenderer.material
                    .SetColor("_EmissionColor", _botsColorPreview);
                server.droneFootballController = footballController.droneFootballController;
                
                footballController.timer.timeForEndGame = _gameTimeInSeconds;
            }
        }

        public void CloseColorPickers()
        {
            botsColorPreview.CloseColorPicker();
            playerColorPreview.CloseColorPicker();
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

        public void BackToMenu()
        {
            SceneManager.LoadScene(0);
        }

        public void Exit()
        {
            // dbManager.SaveUserFootballStatistic();
            // dbManager.SaveUserRaceStatistic();
            // dbManager.SaveUserResolution();
            // dbManager.SaveUserSettings();
            // dbManager.SaveUserData();
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
            GameTimeHandler();
            SceneManager.LoadScene(sceneIndex);
        }

        private void GameTimeHandler()
        {
            if (menuUIManager.gameTimeInput.text.Length > 0)
                _gameTimeInSeconds = Convert.ToInt32(menuUIManager.gameTimeInput.text);
            else
                _gameTimeInSeconds = 300;
        }
    }
}