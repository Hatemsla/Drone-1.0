using UnityEngine;
using TMPro;

namespace Drone.Builder
{
    public class HelpMessage : MonoBehaviour
    {
        private string _messageText;

        [SerializeField] private TMP_Text message;

        public string MessageText
        {
            get => _messageText;
            set
            {
                _messageText = value;
                OnPromptTextChanged();
            }
        }

        private void OnPromptTextChanged()
        {
            message.text = MessageText;
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void ChangeMessageText(string newText)
        {
            MessageText = newText;
        }
    }
}
