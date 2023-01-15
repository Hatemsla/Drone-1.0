using DroneFootball;
using DroneRace;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class MenuManager : MonoBehaviour
    {
        public bool isSimpleMode;
        public MenuUIManager menuUIManager;
        public RaceController raceController;
        public FootballController footballController;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            menuUIManager.raceSimpleModeBtn.onClick.AddListener(delegate { SimpleMode(1); });
            menuUIManager.raceHardModeBtn.onClick.AddListener(delegate { HardMode(1); });
            menuUIManager.footballSimpleModeBtn.onClick.AddListener(delegate { SimpleMode(2); });
            menuUIManager.footballHardModeBtn.onClick.AddListener(delegate { HardMode(2); });
            menuUIManager.exitBtn.onClick.AddListener(Exit);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.buildIndex == 0)
            {
                var dontDestroyOnLoadObjects = FindObjectsOfType<MenuManager>();
                foreach (var obj in dontDestroyOnLoadObjects)
                {
                    if (obj.transform.gameObject != transform.gameObject)
                        Destroy(obj);
                }

                menuUIManager = FindObjectOfType<MenuUIManager>();
                menuUIManager.raceSimpleModeBtn.onClick.AddListener(delegate { SimpleMode(1); });
                menuUIManager.raceHardModeBtn.onClick.AddListener(delegate { HardMode(1); });
                menuUIManager.footballSimpleModeBtn.onClick.AddListener(delegate { SimpleMode(2); });
                menuUIManager.footballHardModeBtn.onClick.AddListener(delegate { HardMode(2); });
                menuUIManager.exitBtn.onClick.AddListener(Exit);
            }
            else if (scene.buildIndex == 1)
            {
                raceController = FindObjectOfType<RaceController>();
                raceController.raceUIManager.backBtn.onClick.AddListener(Back);
                raceController.raceUIManager.exitBtn.onClick.AddListener(Exit);
                raceController.isSimpleMode = isSimpleMode;
            }
            else if (scene.buildIndex == 2)
            {
                footballController = FindObjectOfType<FootballController>();
                footballController.footballUIManager.backBtn.onClick.AddListener(Back);
                footballController.footballUIManager.exitBtn.onClick.AddListener(Exit);
                footballController.isSimpleMode = isSimpleMode;
            }
        }

        public void Back()
        {
            SceneManager.LoadScene(0);
        }

        public void Exit()
        {
            Application.Quit();
        }

        private void SimpleMode(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
            isSimpleMode = true;
        }

        private void HardMode(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
            isSimpleMode = false;
        }
    }
}
