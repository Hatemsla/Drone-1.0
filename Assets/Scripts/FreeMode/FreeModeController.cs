using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace FreeMode
{
    public class FreeModeController : MonoBehaviour
    {
        public float interfaceScale;
        public bool isSimpleMode;
        public bool isGameStart;
        public Transform target;
        public DroneController droneController;
        public FreeModeUIManager freeModeUIManager;
        public Timer timer;

        public Camera mainCamera;
        private CheckNode _checkNode;
        private Vector3 _startPointerSize;

        private void Awake()
        {
            _startPointerSize = freeModeUIManager.pathArrow.sizeDelta;
        }

        private void Start()
        {
            droneController.isSimpleMode = isSimpleMode;
            _checkNode = droneController.checkNode;
            freeModeUIManager.scoreText.text = $"Счет: {_checkNode.passedNode}";
        }

        private void Update()
        {
            CheckStartGame();
            CheckEndGame();
        }

        private void LateUpdate()
        {
            target = _checkNode.nodes[_checkNode.currentNode];
            Vector3 realPos = mainCamera.WorldToScreenPoint(target.position);
            Rect rect = new Rect(0, 0, Screen.width, Screen.height);

            Vector3 outPos = realPos;
            float direction = 1;

            freeModeUIManager.pathArrow.GetComponent<Image>().sprite = freeModeUIManager.outOfScreenIcon;

            if (!IsBehind(target.position)) // если цель спереди
            {
                if (rect.Contains(realPos)) // и если цель в окне экрана
                {
                    freeModeUIManager.pathArrow.GetComponent<Image>().sprite = freeModeUIManager.pointerIcon;
                }
            }
            else // если цель cзади
            {
                realPos = -realPos;
                outPos = new Vector3(realPos.x, 0, 0); // позиция иконки - снизу
                if (mainCamera.transform.position.y < target.position.y)
                {
                    direction = -1;
                    outPos.y = Screen.height; // позиция иконки - сверху				
                }
            }
            // ограничиваем позицию областью экрана
            float offset = freeModeUIManager.pathArrow.sizeDelta.x / 2;
            outPos.x = Mathf.Clamp(outPos.x, offset, Screen.width - offset);
            outPos.y = Mathf.Clamp(outPos.y, offset, Screen.height - offset);

            Vector3 pos = realPos - outPos; // направление к цели из PointerUI 

            RotatePointer(direction * pos);

            freeModeUIManager.pathArrow.sizeDelta = new Vector2(_startPointerSize.x / 100 * interfaceScale, _startPointerSize.y / 100 * interfaceScale);
            freeModeUIManager.pathArrow.anchoredPosition = outPos;
        }

        private bool IsBehind(Vector3 point) // true если point сзади камеры
        {
            Vector3 forward = mainCamera.transform.TransformDirection(Vector3.forward);
            Vector3 toOther = point - mainCamera.transform.position;
            if (Vector3.Dot(forward, toOther) < 0) return true;
            return false;
        }

        private void RotatePointer(Vector2 direction) // поворачивает PointerUI в направление direction
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            freeModeUIManager.pathArrow.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        private void CheckStartGame()
        {
            if (timer.waitForStartGame >= 0)
            {
                freeModeUIManager.timeToStartGameText.text = $"До начала осталось: {timer.waitForStartGame:f1}";
            }
            else
            {
                freeModeUIManager.timeToStartGameText.gameObject.SetActive(false);
                freeModeUIManager.descriptionPanel.SetActive(false);
                isGameStart = true;
            }
        }

        private void CheckEndGame()
        {
            if (timer.waitForEndGame >= 0)
            {
                float minutes = Mathf.FloorToInt(timer.waitForEndGame / 60);
                float seconds = Mathf.FloorToInt(timer.waitForEndGame % 60);
                freeModeUIManager.timeToEndGameText.text = $"{minutes:00}:{seconds:00}";
            }
            else
            {
                if (_checkNode.passedNode == _checkNode.nodes.Count - 1)
                    freeModeUIManager.matchResultText.text = "Вы победили!";

                freeModeUIManager.matchResultPanel.SetActive(true);
                isGameStart = false;
                StartCoroutine(BackToMenu());
            }
        }

        public void CheckScore()
        {
            freeModeUIManager.scoreText.text = $"Счет: {_checkNode.passedNode}";
        }

        private IEnumerator BackToMenu()
        {
            yield return new WaitForSeconds(3);
            freeModeUIManager.backBtn.onClick.Invoke();
        }
    }
}
