using Builder;
using DB;
using DroneFootball;
using DroneRace;
using Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Drone
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public GameData gameData;
        public DBManager dbManager;
        public Server server;
        public ScratchClient scratchClient;
        public RaceController raceController;
        public FootballController footballController;
        public BuilderManager builderManager;
        public AsyncLoad asyncLoad;
        private static readonly int ShowPanel = Animator.StringToHash("ShowPanel");

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            dbManager = GetComponent<DBManager>();
            server = GetComponent<Server>();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            AudioListener.volume = gameData.currentEffectsVolume;
            switch (scene.buildIndex)
            {
                case 1:
                {
                    var gameManagers = FindObjectsOfType<GameManager>();

                    foreach (var gameManager in gameManagers)
                        if(gameManager != this)
                            Destroy(gameManager.gameObject);

                    asyncLoad = FindObjectOfType<AsyncLoad>();
                    InputManager.Instance.TurnCustomActionMap("UI");
                    break;
                }
                case 2:
                    raceController = FindObjectOfType<RaceController>();
                    raceController.raceUIManager.backBtn.onClick.AddListener(delegate
                    {
                        GameManagerUtils.BackToMenu(asyncLoad, raceController.raceUIManager.uiPanel,
                            raceController.raceUIManager.loadPanel);
                    });
                    raceController.raceUIManager.exitBtn.onClick.AddListener(GameManagerUtils.Exit);
                    raceController.isSimpleMode = gameData.isSimpleMode;
                    raceController.droneRaceController.yawPower = gameData.currentYawSensitivity;
                    server.player = raceController.droneRaceController;
                    raceController.currentAIDroneSpeed = gameData.currentAIDroneSpeed;
                    raceController.timer.timeForEndGame = gameData.gameTimeInSeconds;

                    asyncLoad = raceController.asyncLoad;
                    
                    InputManager.Instance.TurnCustomActionMap("Player");
                    break;
                case 3:
                    footballController = FindObjectOfType<FootballController>();
                    footballController.footballUIManager.backBtn.onClick.AddListener(delegate
                    {
                        GameManagerUtils.BackToMenu(asyncLoad, footballController.footballUIManager.uiPanel,
                            footballController.footballUIManager.loadPanel);
                    });
                    footballController.footballUIManager.exitBtn.onClick.AddListener(GameManagerUtils.Exit);
                    footballController.isSimpleMode = gameData.isSimpleMode;
                    footballController.droneFootballController.yawPower = gameData.currentYawSensitivity;
                    server.player = footballController.droneFootballController;
                    footballController.currentGateScale = gameData.currentGateScale;
                    footballController.currentAIDroneSpeed = gameData.currentAIDroneSpeed;
                    footballController.timer.timeForEndGame = gameData.gameTimeInSeconds;

                    asyncLoad = footballController.asyncLoad;
                    
                    InputManager.Instance.TurnCustomActionMap("Player");
                    break;
                case 4:
                {
                    builderManager = FindObjectOfType<BuilderManager>();
                    builderManager.currentYawSensitivity = gameData.currentYawSensitivity;
                    builderManager.builderUI.editorExitBtn.onClick.AddListener(GameManagerUtils.Exit);
                    builderManager.builderUI.gameExitBtn.onClick.AddListener(GameManagerUtils.Exit);
                    builderManager.builderUI.backBtn.onClick.AddListener(delegate
                    {
                        GameManagerUtils.BackToMenu(asyncLoad, builderManager.builderUI.uiPanel,
                            builderManager.builderUI.loadPanel);
                    });
                    builderManager.builderUI.backEditorTabBtn.onClick.AddListener(delegate
                    {
                        GameManagerUtils.BackToMenu(asyncLoad, builderManager.builderUI.uiPanel,
                            builderManager.builderUI.loadPanel);
                    });
                    builderManager.builderUI.backGameTabBtn.onClick.AddListener(delegate
                    {
                        GameManagerUtils.BackToMenu(asyncLoad, builderManager.builderUI.uiPanel,
                            builderManager.builderUI.loadPanel);
                    });
                    builderManager.builderUI.saveBtn.onClick.AddListener(SaveLevel);
                    builderManager.levelName = gameData.levelName;
                    builderManager.droneBuilderController.isSimpleMode = gameData.isSimpleMode;
                    server.player = builderManager.droneBuilderController;
                    asyncLoad = builderManager.asyncLoad;

                    if (gameData.isLoadLevel)
                    {
                        // builderManager.LoadScene();
                        builderManager.isGameMode = false;
                        builderManager.isLoadLevel = gameData.isLoadLevel;
                    }

                    if (gameData.isStartBuilder)
                    {
                        // builderManager.StartLevel();
                        builderManager.isGameMode = true;
                        builderManager.isGameLevel = true;
                        InputManager.Instance.TurnCustomActionMap("Player");
                        return;
                    }
                    
                    InputManager.Instance.TurnCustomActionMap("Builder");

                    break;
                }
            }
        }

        private void SaveLevel()
        {
            LevelManager.SaveLevel(builderManager, gameData.levelName);
            builderManager.builderUI.savePanelAnimator.SetTrigger(ShowPanel);
        }

        public void GetScratchData(ScratchData droneData)
        {
            switch (droneData.Mode)
            {
                case "AltHold":
                    SetAltholdMode(droneData);
                    break;
                case "Angle":
                    SetAngleMode(droneData);
                    break;
                case "Ручной":
                    break;
            }
        }
        
        private void SetAngleMode(ScratchData droneData)
        {
            if (footballController)
            {
                footballController.droneFootballController.isSimpleMode = false;
                footballController.droneFootballController.cyclic =
                    new Vector2(droneData.Roll / 100, droneData.Pitch / 100);
                footballController.droneFootballController.pedals = droneData.Yaw / 100;
                footballController.droneFootballController.throttle = droneData.Angle / 100;
                return;
            }

            if (raceController)
            {
                raceController.droneRaceController.isSimpleMode = false;
                raceController.droneRaceController.cyclic =
                    new Vector2(droneData.Roll / 100, droneData.Pitch / 100);
                raceController.droneRaceController.pedals = droneData.Yaw / 100;
                raceController.droneRaceController.throttle = droneData.Angle / 100;
                return;
            }

            if (builderManager)
            {
                builderManager.droneBuilderController.isSimpleMode = false;
                builderManager.droneBuilderController.cyclic =
                    new Vector2(droneData.Roll / 100, droneData.Pitch / 100);
                builderManager.droneBuilderController.pedals = droneData.Yaw / 100;
                builderManager.droneBuilderController.throttle = droneData.Angle / 100;
            }
        }

        private void SetAltholdMode(ScratchData droneData)
        {
            if (footballController)
            {
                footballController.droneFootballController.isSimpleMode = true;
                footballController.droneFootballController.cyclic =
                    new Vector2(droneData.Roll / 100, droneData.Pitch / 100);
                footballController.droneFootballController.pedals = droneData.Yaw / 100;
                footballController.droneFootballController.throttle = droneData.Althold / 100;
                return;
            }

            if (raceController)
            {
                raceController.droneRaceController.isSimpleMode = true;
                raceController.droneRaceController.cyclic =
                    new Vector2(droneData.Roll / 100, droneData.Pitch / 100);
                raceController.droneRaceController.pedals = droneData.Yaw / 100;
                raceController.droneRaceController.throttle = droneData.Althold / 100;
                return;
            }

            if (builderManager)
            {
                builderManager.droneBuilderController.isSimpleMode = true;
                builderManager.droneBuilderController.cyclic =
                    new Vector2(droneData.Roll / 100, droneData.Pitch / 100);
                builderManager.droneBuilderController.pedals = droneData.Yaw / 100;
                builderManager.droneBuilderController.throttle = droneData.Althold / 100;
            }
        }
    }
}