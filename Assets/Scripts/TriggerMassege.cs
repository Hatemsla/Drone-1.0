using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Drone;

namespace Builder
{
    public class TriggerMassege : InteractiveObject
    {
        [SerializeField] private HelpMessage message;
        public GameObject TriggerObject;
        public string messageText;

        private bool FirstEnter = true;

        void Start()
        {
        
        }

        void Update()
        {
        
        }

        private void FindMassege()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (!FirstEnter || !BuilderManager.Instance.isMove)
            {
                return;
            }
            if (other.GetComponentInParent<DroneController>() is DroneBuilderController drone)
            {
                message.SetActive(true);
                message.ChangeMessageText(messageText);
                FirstEnter = false;
            }
        }

        private void ShowMassage()
        {

        }

        private void FindMassge()
        {
            message = FindObjectOfType<BuilderUI>().helpMessage;
        }

        private void OnEnable()
        {
            Debug.Log("OnEnable");
            BuilderManager.Instance.ObjectChangeSceneEvent += FindMassge;
        }

        private void OnDisable()
        {
            Debug.Log("OnDisable");
            BuilderManager.Instance.ObjectChangeSceneEvent -= FindMassge;
        }

        private void SetMessageText(string value)
        {
            messageText = value;
        }

        public override void SetActive(bool active)
        {
        }

        public override void SetColorIndex(int active)
        {
        }
    }
}
