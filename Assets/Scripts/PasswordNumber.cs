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

        public void StartFading()
        {
            StartCoroutine(FadeText());
        }

        public void StopFading()
        {
            passwordNumber.color = _startNumberColor;
        }

        private IEnumerator FadeText()
        {
            var duration = 1.0f; // Измените значение, если хотите увеличить или уменьшить время анимации
            var startAlpha = 1.0f; // Начальная прозрачность (100%)
            var targetAlpha = 0.2f; // Конечная прозрачность (50%)

            var startColor = passwordNumber.color;

            while (isSelected)
            {
                // Прямая анимация от 100 до 50
                float currentTime = 0;
                while (currentTime < duration)
                {
                    currentTime += Time.deltaTime;
                    var alpha = Mathf.Lerp(startAlpha, targetAlpha, currentTime / duration);
                    passwordNumber.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                    yield return null;
                }

                // Обратная анимация от 50 до 100
                currentTime = 0;
                while (currentTime < duration)
                {
                    currentTime += Time.deltaTime;
                    var alpha = Mathf.Lerp(targetAlpha, startAlpha, currentTime / duration);
                    passwordNumber.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                    yield return null;
                }
            }

            // Убедимся, что прозрачность установлена на окончательное значение (100%), когда isSelected становится ложным
            passwordNumber.color = startColor;
        }
    }
}