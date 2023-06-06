using System.Text;
using Builder;
using DB;
using DroneRace;
using Menu;
using Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DroneFootball
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public GameData gameData;
        public DBManager dbManager;
        public Server server;
        public RaceController raceController;
        public FootballController footballController;
        public BuilderManager builderManager;
        public AsyncLoad asyncLoad;

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
            AudioListener.volume = gameData.currentVolume;
            switch (scene.buildIndex)
            {
                case 1:
                {
                    var dontDestroyGameManager = FindObjectsOfType<GameManager>();
                    var dontDestroyDbManager = FindObjectsOfType<DBManager>();
                    var dontDestroyServer = FindObjectsOfType<Server>();
                
                    foreach (var obj in dontDestroyGameManager)
                        if (obj.transform.gameObject != transform.gameObject)
                            Destroy(obj);

                    foreach (var obj in dontDestroyDbManager)
                        if (obj.transform.gameObject != transform.gameObject)
                            Destroy(obj);

                    foreach (var obj in dontDestroyServer)
                        if (obj.transform.gameObject != transform.gameObject)
                            Destroy(obj);

                    asyncLoad = FindObjectOfType<AsyncLoad>();
                    break;
                }
                case 2:
                    raceController = FindObjectOfType<RaceController>();
                    raceController.raceUIManager.backBtn.onClick.AddListener(delegate { GameManagerUtils.BackToMenu(asyncLoad, raceController.raceUIManager.uiPanel, raceController.raceUIManager.loadPanel); });
                    raceController.raceUIManager.exitBtn.onClick.AddListener(GameManagerUtils.Exit);
                    raceController.isSimpleMode = gameData.isSimpleMode;
                    raceController.droneRaceController.yawPower = gameData.currentYawSensitivity;
                    server.droneRaceController = raceController.droneRaceController;
                    raceController.currentAIDroneSpeed = gameData.currentAIDroneSpeed;
                    raceController.timer.timeForEndGame = gameData.gameTimeInSeconds;
                
                    asyncLoad = raceController.asyncLoad;
                    break;
                case 3:
                    footballController = FindObjectOfType<FootballController>();
                    footballController.footballUIManager.backBtn.onClick.AddListener(delegate { GameManagerUtils.BackToMenu(asyncLoad, footballController.footballUIManager.uiPanel, footballController.footballUIManager.loadPanel); });
                    footballController.footballUIManager.exitBtn.onClick.AddListener(GameManagerUtils.Exit);
                    footballController.isSimpleMode = gameData.isSimpleMode;
                    footballController.droneFootballController.yawPower = gameData.currentYawSensitivity;
                    server.droneFootballController = footballController.droneFootballController;
                    footballController.currentGateScale = gameData.currentGateScale;
                    footballController.currentAIDroneSpeed = gameData.currentAIDroneSpeed;
                    footballController.timer.timeForEndGame = gameData.gameTimeInSeconds;
                
                    asyncLoad = footballController.asyncLoad;
                    break;
                case 4:
                {
                    builderManager = FindObjectOfType<BuilderManager>();
                    builderManager.currentYawSensitivity = gameData.currentYawSensitivity;
                    builderManager.builderUI.editorExitBtn.onClick.AddListener(GameManagerUtils.Exit);
                    builderManager.builderUI.gameExitBtn.onClick.AddListener(GameManagerUtils.Exit);
                    builderManager.builderUI.backBtn.onClick.AddListener(delegate { GameManagerUtils.BackToMenu(asyncLoad, builderManager.builderUI.uiPanel, builderManager.builderUI.loadPanel); });
                    builderManager.builderUI.backEditorTabBtn.onClick.AddListener(delegate { GameManagerUtils.BackToMenu(asyncLoad, builderManager.builderUI.uiPanel, builderManager.builderUI.loadPanel); });
                    builderManager.builderUI.backGameTabBtn.onClick.AddListener(delegate { GameManagerUtils.BackToMenu(asyncLoad, builderManager.builderUI.uiPanel, builderManager.builderUI.loadPanel); });
                    builderManager.builderUI.saveBtn.onClick.AddListener(delegate { LevelManager.SaveLevel(builderManager, gameData.levelName); });
                    builderManager.levelName = gameData.levelName;
                    builderManager.droneBuilderController.isSimpleMode = gameData.isSimpleMode; 
                    server.droneBuilderController = builderManager.droneBuilderController;
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
                    }

                    break;
                }
            }
        }
    }
}