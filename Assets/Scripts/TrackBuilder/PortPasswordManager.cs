using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Builder;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Drone
{
    public class PortPasswordManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text hintText;
        [SerializeField] private TMP_Text alarmText;
        [SerializeField] private TMP_Text entreText;
        [SerializeField] private Image closeImage;
        [SerializeField] private Image hintImage;
        [SerializeField] private Image infoImage;
        [SerializeField] private Image entreImage;
        [SerializeField] private Image backImage;
        [SerializeField] private Image lightningImage;
        [SerializeField] private Sprite closeBlue;
        [SerializeField] private Sprite closeYellow;
        [SerializeField] private Sprite closeRed;
        [SerializeField] private Sprite infoBlue;
        [SerializeField] private Sprite infoYellow;
        [SerializeField] private Sprite infoRed;
        [SerializeField] private Sprite backBlue;
        [SerializeField] private Sprite backYellow;
        [SerializeField] private Sprite backRed;
        [SerializeField] private Sprite entreBlue;
        [SerializeField] private Sprite entreYellow;
        [SerializeField] private Sprite entreRed;
        [SerializeField] private Sprite hintBlue;
        [SerializeField] private Sprite hintYellow;
        [SerializeField] private Sprite hintRed;
        [SerializeField] private Sprite lightningBlue;
        [SerializeField] private Sprite lightningYellow;
        [SerializeField] private Sprite lightningRed;
        
        [SerializeField] private PasswordNumber[] numbers;
        [SerializeField] private Image[] arrows;
        public PortPassword currentPortPassword;
        
        private List<PortPassword> _portPasswords = new();
        private int _selectedNumberIndex;
        private int[] _numberCounters;

        private void Start()
        {
            _numberCounters = new int[numbers.Length];
            BuilderManager.Instance.TestLevelEvent += GetPortPasswords;
        }

        private void OnEnable()
        {
            InputManager.Instance.ExitPasswordEvent += ClosePassword;
            InputManager.Instance.SpinNumberYUpEvent += SpinNumberUp;
            InputManager.Instance.SpinNumberYDownEvent += SpinNumberDown;
            InputManager.Instance.SpinNumberXLeftEvent += SpinNumberLeft;
            InputManager.Instance.SpinNumberXRightEvent += SpinNumberRight;
        }

        private void OnDestroy()
        {
            InputManager.Instance.ExitPasswordEvent -= ClosePassword;
            InputManager.Instance.SpinNumberYUpEvent -= SpinNumberUp;
            InputManager.Instance.SpinNumberYDownEvent -= SpinNumberDown;
            InputManager.Instance.SpinNumberXLeftEvent -= SpinNumberLeft;
            InputManager.Instance.SpinNumberXRightEvent -= SpinNumberRight;
        }
        
        private void GetPortPasswords()
        {
            if(!BuilderManager.Instance.isMove)
                return;

            if (_portPasswords.Count > 0)
            {
                foreach (var port in _portPasswords)
                {
                    port.OpenPasswordEvent -= GetPassword;
                }
            }
            
            _portPasswords = FindObjectsOfType<PortPassword>().ToList();
            foreach (var port in _portPasswords)
            {
                port.OpenPasswordEvent += GetPassword;
            }
        }

        private void GetPassword(PortPassword portPassword)
        {
            currentPortPassword = portPassword;
            UpdatePasswordUI(currentPortPassword.PasswordInputLoss);
        }

        public void CheckPassword()
        {
            if(currentPortPassword.IsBlocked())
                return;
            
            if(currentPortPassword.CheckPassword($"{numbers[0].Number}{numbers[1].Number}{numbers[2].Number}"))
                OpenPort();
            else
                GetDanger();
        }

        private void GetDanger()
        {
            var countLosses = currentPortPassword.IncorrectPassword();

            UpdatePasswordUI(countLosses);
        }

        private void SelectArrow()
        {
            for (var i = 0; i < arrows.Length; i++)
                arrows[i].gameObject.SetActive(i == _selectedNumberIndex);
        }

        private void SetArrowsColor(Color color)
        {
            foreach (var arrow in arrows)
                arrow.color = color;
        }

        private void UpdatePasswordUI(int countLosses)
        {
            switch (countLosses)
            {
                case 3:
                {
                    var blue = TrackBuilderUtils.HexToColor(Idents.HexColors.Blue8Cf2F4);
                    alarmText.color = blue;
                    entreText.color = blue;
                    hintText.text = "Если Вы трижды вводите неверный ПИН-КОД - автоматически запускается сигнал тревоги. ";
                    entreText.text = "Войти";
                    numbers[0].SetNumberColor(blue);
                    numbers[1].SetNumberColor(blue);
                    numbers[2].SetNumberColor(blue);
                    SetArrowsColor(blue);
                    entreImage.sprite = entreBlue;
                    backImage.sprite = backBlue;
                    lightningImage.sprite = lightningBlue;
                    infoImage.sprite = infoBlue;
                    closeImage.sprite = closeBlue;
                    hintImage.sprite = hintBlue;
                    break;
                }
                case 2:
                {
                    var yellow = TrackBuilderUtils.HexToColor(Idents.HexColors.YellowFaff00);
                    alarmText.color = yellow;
                    entreText.color = yellow;
                    hintText.text = "У Вас осталось 2 попытки";
                    entreText.text = "Доступ ЗАПРЕЩЁН";
                    numbers[0].SetNumberColor(yellow);
                    numbers[1].SetNumberColor(yellow);
                    numbers[2].SetNumberColor(yellow);
                    SetArrowsColor(yellow);
                    entreImage.sprite = entreYellow;
                    backImage.sprite = backYellow;
                    lightningImage.sprite = lightningYellow;
                    infoImage.sprite = infoYellow;
                    closeImage.sprite = closeYellow;
                    hintImage.sprite = hintYellow;
                    break;
                }
                case 1:
                    hintText.text =
                        "У Вас осталась 1 попытка, и если ПИН-КОД будет введён неправильно, будет объявлена тревога!";
                    break;
                default:
                {
                    var red = TrackBuilderUtils.HexToColor(Idents.HexColors.RedFf0000);
                    alarmText.color = red;
                    entreText.color = red;
                    hintText.text = "ОБЪЯВЛЕНА ТРЕВОГА!!!";
                    numbers[0].SetNumberColor(red);
                    numbers[1].SetNumberColor(red);
                    numbers[2].SetNumberColor(red);
                    SetArrowsColor(red);
                    entreImage.sprite = entreRed;
                    backImage.sprite = backRed;
                    lightningImage.sprite = lightningRed;
                    infoImage.sprite = infoRed;
                    closeImage.sprite = closeRed;
                    hintImage.sprite = hintRed;
                    currentPortPassword.BlockPort();
                    break;
                }
            }
        }

        private void OpenPort()
        {
            var currentPort = currentPortPassword.GetComponent<Port>();
            currentPort.OpenPortAfterPassword();
        }

        private void SpinNumberRight()
        {
            if (_selectedNumberIndex == 2)
                _selectedNumberIndex = 0;
            else
                _selectedNumberIndex++;


            SelectArrow();
        }

        private void SpinNumberLeft()
        {
            if (_selectedNumberIndex == 0)
                _selectedNumberIndex = 2;
            else
                _selectedNumberIndex--;

            SelectArrow();
        }

        private void SpinNumberDown()
        {
            var prevNumber = --_numberCounters[_selectedNumberIndex];
            if (prevNumber < 0)
                prevNumber = 9;
            
            _numberCounters[_selectedNumberIndex] = prevNumber;
            
            numbers[_selectedNumberIndex].Number = prevNumber;
        }

        private void SpinNumberUp()
        {
            var nextNumber = ++_numberCounters[_selectedNumberIndex];
            if (nextNumber > 9)
                nextNumber = 0;

            _numberCounters[_selectedNumberIndex] = nextNumber;
            
            numbers[_selectedNumberIndex].Number = nextNumber;
        }

        private void ClosePassword()
        {
            InputManager.Instance.TurnCustomActionMap(Idents.ActionMaps.Player);
            BuilderManager.Instance.builderUI.droneView.SetActive(true);
            BuilderManager.Instance.builderUI.passwordUI.SetActive(false);
        }
    }
}