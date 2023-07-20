using System;
using TMPro;
using UnityEngine;

namespace Drone
{
    public sealed class Prompt : MonoBehaviour
    {
        private string _promptText;
        
        [SerializeField] private TMP_Text prompt;
        
        public string PromptText
        {
            get => _promptText;
            set
            {
                _promptText = value;
                OnPromptTextChanged();
            }
        }

        private void OnPromptTextChanged()
        {
            prompt.text = PromptText;
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}