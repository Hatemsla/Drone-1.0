using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Drone;

namespace Drone.Builder
{
    public class TriggerMassege : InteractiveObject
    {
        [SerializeField] private HelpMessage message;
        public GameObject TriggerObject;
        private Renderer objectRenderer;
        public ColorOption selectedColorOption;
        private MeshRenderer mesh;
        private bool FirstEnter;


        float openTime;

        void Start()
        {
            FirstEnter = true;
            objectRenderer = TriggerObject.GetComponent<Renderer>();
            mesh = TriggerObject.GetComponent<MeshRenderer>();
            SetColor(GetColorFromOption(selectedColorOption));  

        }

        void Update()
        {
            if (BuilderManager.Instance.isMove)
            {
                mesh.enabled = false;
            }
            else
            {
                mesh.enabled = true;
            }
            // Debug.Log((openTime - Time.deltaTime));
            if ((openTime - Time.deltaTime) < -5)
            {
                
                message.SetActive(false);
            }

        }

        private void SetColor(Color scolor)
        {   
            Color newColor = new Color(scolor.r, scolor.g, scolor.b, 0.3529412f);
            objectRenderer.material.SetColor("_Color", newColor);

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
                message.ChangeMessageText(text3D);
                FirstEnter = false;
                StartCoroutine(closeMessage());


            }
        }

        IEnumerator closeMessage()
        {

            yield return new WaitForSeconds(5);

            message.SetActive(false);

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
            text3D = value;
        }

        public override void SetActive(bool active)
        {

        }

        public override void SetColorIndex(int value)
        {
            color_index = value;
            selectedColorOption = (ColorOption)value;
            SetColor(GetColorFromOption(selectedColorOption));
        }
    }
}
