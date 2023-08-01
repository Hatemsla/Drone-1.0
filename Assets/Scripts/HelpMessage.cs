using UnityEngine;
using TMPro;

namespace Builder
{
    public class HelpMessage : MonoBehaviour
    {
        private string _massgeText;

        [SerializeField] private TMP_Text message;

        public string MassgeText
        {
            get => _massgeText;
            set
            {
                _massgeText = value;
                OnPromptTextChanged();
            }
        }

        private void OnPromptTextChanged()
        {
            message.text = MassgeText;
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void ChangeMessageText(string newText)
        {
            MassgeText = newText;
        }
    }
}
