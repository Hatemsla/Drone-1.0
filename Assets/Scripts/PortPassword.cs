using System;
using Builder;
using UnityEngine;

namespace Drone
{
    public class PortPassword : MonoBehaviour
    {
        [SerializeField] private string password = "777";

        public string Password
        {
            get => password;
            set => password = value;
        }
        
        public event Action<PortPassword> OpenPasswordEvent;

        public void OpenPassword()
        {
            OpenPasswordEvent?.Invoke(this);
            InputManager.Instance.TurnCustomActionMap(Idents.ActionMaps.PortPassword);
            BuilderManager.Instance.builderUI.droneView.SetActive(false);
            BuilderManager.Instance.builderUI.prompt.SetActive(false);
            BuilderManager.Instance.builderUI.passwordUI.SetActive(true);
        }

        public bool CheckPassword(string inputPassword)
        {
            return inputPassword == password;
        }
    }
}