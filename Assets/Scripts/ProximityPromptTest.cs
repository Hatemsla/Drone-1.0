using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace Builder
{
    public class ProximityPromptTest : MonoBehaviour
    {
        public GameObject promptTextObject;
        private Text promptText;
        public Camera mainCamera;
        private Camera[] additionalCameras;
        private int currentCameraIndex = 0;        
        private bool isMainCamera = true;
        private int mainCameraIndex;
        private int nowCameraIndex; 
        private GameObject myObject; 
        private BoxCollider[] colliders;
        private bool previousIsMove;


        void Start()
        {
            myObject = gameObject;
            mainCamera = Camera.main;
            promptTextObject = GameObject.Find("PromptText");
            promptText = promptTextObject.GetComponent<Text>();
            promptText.enabled = false;
            GameObject[] cameraObjects = GameObject.FindGameObjectsWithTag("SecurityCamera");
            additionalCameras = new Camera[cameraObjects.Length];
            for (int i = 0; i < cameraObjects.Length; i++)
            {
                additionalCameras[i] = cameraObjects[i].GetComponent<Camera>();
            }
            Debug.Log("Найдено " + additionalCameras.Length + " камер(ы) безопасности.");
            DeactivateAllCameras();  
            mainCamera.enabled = true;         
            nowCameraIndex = 0;
            colliders = myObject.GetComponentsInChildren<BoxCollider>();                      
        }

        void Update()
        {
            // ChangeTrigger();

            if (promptText.enabled)
            {
                CheckButton();
                
            }
        }

        private void ChangeTrigger()
        {
            if (previousIsMove != BuilderManager.Instance.isMove)            
            {
                previousIsMove = BuilderManager.Instance.isMove;
                foreach (BoxCollider collider in colliders)
                {
                    collider.enabled = BuilderManager.Instance.isMove;
                }
            }

        }

        private void CheckButton()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                isMainCamera = !isMainCamera;
                Debug.Log("Switch Camera");
                if (isMainCamera)
                {     
                    Debug.Log("Main Camera");       
                    additionalCameras[nowCameraIndex].enabled = false;
                    mainCamera.enabled = true;
                }
                else
                {
                    NextCameraActivation();  
                } 
                Debug.Log(nowCameraIndex);   
            }
            if (!isMainCamera)
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    PreviousCameraActivation();                    
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    NextCameraActivation();
                }
            }
        } 

        private void NextCameraActivation()
        {
            currentCameraIndex++;            
            if (currentCameraIndex >= additionalCameras.Length)
                {
                    currentCameraIndex = 0;
                }
            additionalCameras[nowCameraIndex].enabled = false;
            additionalCameras[currentCameraIndex].enabled = true;
            nowCameraIndex = currentCameraIndex;
        }

        private void PreviousCameraActivation()
        {
            currentCameraIndex--;
            if (currentCameraIndex < 0)
                {
                    currentCameraIndex = additionalCameras.Length - 1;
                }
            additionalCameras[nowCameraIndex].enabled = false;
            additionalCameras[currentCameraIndex].enabled = true;
            nowCameraIndex = currentCameraIndex;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                promptText.enabled = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                promptText.enabled = false;
            }
        }

        void DeactivateAllCameras()
        {            
            foreach (var camera in additionalCameras)
            {
                camera.enabled = false;
            }
        }
    }
}
