using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Drone.Builder
{
    public class PasswordNumber : MonoBehaviour
    {
        [SerializeField] private TMP_Text passwordNumber;

        private int _number;

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

        public void SetNumberColor(Color color)
        {
            passwordNumber.color = color;
        }
    }
}