using TMPro;
using UnityEngine;

namespace Builder
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
                prompt.text = value;
            }
        }
        
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}