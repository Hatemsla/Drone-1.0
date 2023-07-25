using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Drone
{
    public class PasswordNumber : MonoBehaviour
    {
        public bool isSelected;
        [SerializeField] private TMP_Text passwordNumber;

        private int _number;
        private Color _startNumberColor;

        public int Number
        {
            get => _number;
            set
            {
                _number = value switch
                {
                    > 9 => 9,
                    < 0 => 0,
                    _ => value
                };
                passwordNumber.text = _number.ToString();
            }
        }

        private void Start()
        {
            _startNumberColor = passwordNumber.color;
        }

        public void SetNumberColor(Color color)
        {
            passwordNumber.color = color;
        }
    }
}