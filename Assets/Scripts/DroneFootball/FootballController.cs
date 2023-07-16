using System.Collections;
using Cinemachine;
using DB;
using Drone;
using Menu;
using UnityEngine;
using UnityEngine.UI;

namespace DroneFootball
{
    public class FootballController : MonoBehaviour
    {
        public float interfaceScale;
        public float currentGateScale;
        public float currentAIDroneSpeed;
        public bool isSimpleMode;
        public bool isGameStart;
        public Transform targetCheckpoint;
        public DroneController droneFootballController;
        public CheckNode playerCheckNode;
        public DroneFootballSoundController droneFootballSoundController;
        public FootballUIManager footballUIManager;
        public Timer timer;
        public AsyncLoad asyncLoad;
        public DroneFootballAI[] droneFootballAIList;
        public FootballCheckpointTrigger footballCheckpointTrigger;
        public CinemachineBrain playerCamera;

        private Vector3 _startPointerSize;
        private bool _isResult;
        private bool _isTabPanel;

        private void Awake()
        {
            _startPointerSize = footballUIManager.pathArrow.sizeDelta;
        }

        private void Start()
        {
            footballCheckpointTrigger.currentGateScale = currentGateScale;
            droneFootballController.isSimpleMode = isSimpleMode;
            playerCheckNode = droneFootballController.GetComponent<CheckNode>();

            foreach (var droneAI in droneFootballAIList)
            {
                droneAI.footballController = this;
                droneAI.speed *= currentAIDroneSpeed;
            }

            playerCamera = FindObjectOfType<CinemachineBrain>();
        }
        
        private void OnEnable()
        {
            InputManager.Instance.ExitEvent += CheckTabPanel;
        }

        private void OnDisable()
        {
            InputManager.Instance.ExitEvent -= CheckTabPanel;
        }

        private void Update()
        {
            CheckStartGame();
            CheckEndGame();
            CheckScore();
        }

        private void LateUpdate()
        {
            Vector3 realPos = playerCamera.OutputCamera.WorldToScreenPoint(targetCheckpoint.position);
            Rect rect = new Rect(0, 0, Screen.width, Screen.height);

            Vector3 outPos = realPos;
            float direction = 1;

            footballUIManager.pathArrow.GetComponent<Image>().sprite = footballUIManager.outOfScreenIcon;

            if (!IsBehind(targetCheckpoint.position)) // если цель спереди
            {
                if (rect.Contains(realPos)) // и если цель в окне экрана
                {
                    footballUIManager.pathArrow.GetComponent<Image>().sprite = footballUIManager.pointerIcon;
                }
            }
            else // если цель cзади
            {
                realPos = -realPos;
                outPos = new Vector3(realPos.x, 0, 0); // позиция иконки - снизу
                if (playerCamera.transform.position.y < targetCheckpoint.position.y)
                {
                    direction = -1;
                    outPos.y = Screen.height; // позиция иконки - сверху				
                }
            }
            // ограничиваем позицию областью экрана
            float offset = footballUIManager.pathArrow.sizeDelta.x / 2;
            outPos.x = Mathf.Clamp(outPos.x, offset, Screen.width - offset);
            outPos.y = Mathf.Clamp(outPos.y, offset, Screen.height - offset);

            Vector3 pos = realPos - outPos; // направление к цели из PointerUI 

            RotatePointer(direction * pos);

            footballUIManager.pathArrow.sizeDelta = new Vector2(_startPointerSize.x / 100 * interfaceScale, _startPointerSize.y / 100 * interfaceScale);
            footballUIManager.pathArrow.position = outPos;
        }

        private bool IsBehind(Vector3 point) // true если point сзади камеры
        {
            var forward = playerCamera.transform.TransformDirection(Vector3.forward);
            var toOther = point - playerCamera.transform.position;
            if (Vector3.Dot(forward, toOther) < 0) return true;
            return false;
        }

        private void RotatePointer(Vector2 direction) // поворачивает PointerUI в направление direction
        {
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            footballUIManager.pathArrow.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        private void CheckTabPanel()
        {
            _isTabPanel = !_isTabPanel;
            if(_isTabPanel)
                droneFootballSoundController.droneFly.Stop();
            else if(isGameStart)
                droneFootballSoundController.droneFly.Play();
            footballUIManager.tabPanel.SetActive(_isTabPanel);
            Time.timeScale = _isTabPanel ? 0f : 1f;
        }

        private void CheckStartGame()
        {
            if (timer.waitForStartGame >= 0)
            {
                footballUIManager.timeToStartGameText.text = $"00:{timer.waitForStartGame:00}";
            }
            else
            {
                footballUIManager.timeToStartGameText.gameObject.SetActive(false);
                footballUIManager.descriptionPanel.SetActive(false);
                footballUIManager.backgroundImage.SetActive(false);
                footballUIManager.droneView.SetActive(true);
                isGameStart = true;
                if(!droneFootballSoundController.droneFly.isPlaying && !_isTabPanel)
                    droneFootballSoundController.droneFly.Play();
            }
        }

        private void CheckEndGame()
        {
            if (timer.waitForEndGame >= 0)
            {
                float minutes = Mathf.FloorToInt(timer.waitForEndGame / 60);
                float seconds = Mathf.FloorToInt(timer.waitForEndGame % 60);
                footballUIManager.timeText.text = $"{minutes:00}:{seconds:00}";
                footballUIManager.speedText.text = $"{droneFootballController.currentSpeed:00}";
            }
            else
            {
                if (!_isResult)
                {
                    var db = FindObjectOfType<DBManager>();
                    if (playerCheckNode.currentNode >= 10)
                    {
                        footballUIManager.matchResultText.text = "Вы победили!";
                        db.UserStatisticFootball.GamesCount++;
                        db.UserStatisticFootball.WinsCount++;
                    }
                    else
                    {
                        footballUIManager.matchResultText.text = "Вы проиграли(";
                        db.UserStatisticFootball.GamesCount++;
                        db.UserStatisticFootball.LosesCount++;
                    }

                    footballUIManager.matchResultPanel.SetActive(true);
                    isGameStart = false;
                    StartCoroutine(BackToMenu());
                    _isResult = true;
                }
            }
        }

        public void CheckScore()
        {
            footballUIManager.checkpointsCountText.text = $"{playerCheckNode.currentNode}";
        }

        public void TurnUI()
        {
            footballUIManager.droneView.SetActive(false);
            footballUIManager.uiPanel.SetActive(false);
        }

        private IEnumerator BackToMenu()
        {
            yield return new WaitForSeconds(3);
            footballUIManager.backBtn.onClick.Invoke();
        }
    }
}