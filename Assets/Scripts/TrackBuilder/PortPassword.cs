using System;
using System.Collections;
using System.Collections.Generic;
using Drone.Builder;
using UnityEngine;

namespace Drone
{
    public class PortPassword : MonoBehaviour
    {
        [SerializeField] private string password = "777";
        [SerializeField] private int passwordInputLoss = 3;
        [SerializeField] private bool isBlocked;

        public string Password
        {
            get => password;
            set => password = value;
        }

        public int PasswordInputLoss
        {
            get => passwordInputLoss;
            set => passwordInputLoss = value;
        }

        public bool IsBlocked() => isBlocked;
        
        public event Action<PortPassword> OpenPasswordEvent;

        public void OpenPassword()
        {
            if(isBlocked)
                return;

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

        public int IncorrectPassword()
        {
            return --PasswordInputLoss;
        }
        
        public void BlockPort()
        {
            isBlocked = true;
            StartCoroutine(WaitToUnblockPort());
        }
        
        public void UnblockPort()
        {
            isBlocked = false;
        }
        
        private IEnumerator WaitToUnblockPort()
        {
            yield return new WaitForSeconds(30f);
            UnblockPort();
            PasswordInputLoss = 3;
        }
    }
}