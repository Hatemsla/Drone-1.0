using System;
using System.Collections.Generic;
using System.Linq;
using Builder;
using UnityEngine;

namespace Drone
{
    public class PortPasswordManager : MonoBehaviour
    {
        [SerializeField] private PasswordNumber[] numbers;
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
        }

        public void CheckPassword()
        {
            if(currentPortPassword.CheckPassword($"{numbers[0].Number}{numbers[1].Number}{numbers[2].Number}"))
                OpenPort();
            else
                GetDanger();
        }

        private void GetDanger()
        {
            Debug.Log("Get danger");
        }

        private void OpenPort()
        {
            var currentPort = currentPortPassword.GetComponent<Port>();
            currentPort.OpenPortAfterPassword();
        }

        private void StartNumberFade()
        {
            for (var i = 0; i < numbers.Length; i++)
            {
                if (i == _selectedNumberIndex)
                {
                    numbers[i].isSelected = true;
                    numbers[i].StartFading();
                }
                else
                {
                    numbers[i].isSelected = false;
                    numbers[i].StopFading();
                }
            }
        }
        
        private void SpinNumberRight()
        {
            if (_selectedNumberIndex == 2)
                _selectedNumberIndex = 0;
            else
                _selectedNumberIndex++;
            

            StartNumberFade();
        }

        private void SpinNumberLeft()
        {
            if (_selectedNumberIndex == 0)
                _selectedNumberIndex = 2;
            else
                _selectedNumberIndex--;

            StartNumberFade();
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