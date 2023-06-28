using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace Drone
{
    public class ProximityPromptTest : MonoBehaviour
    {
        public GameObject promptTextObject;
        private Text promptText;
        public Camera mainCamera;
        // public Camera watchCamera;
        private Camera[] additionalCameras;
        // private Camera[] additionalCameras;
        private int currentCameraIndex = 0;        
        private bool isMainCamera = true;
        private int mainCameraIndex;
        private int nowCameraIndex; 


        // Start is called before the first frame update
        void Start()
        {
            mainCamera = Camera.main;
            promptTextObject = GameObject.Find("PromptText");
            promptText = promptTextObject.GetComponent<Text>();
            promptText.enabled = false;            
            additionalCameras = FindObjectsOfType<Camera>(); 
            DeactivateAllCameras();  
            mainCamera.enabled = true;         
            mainCameraIndex = FindMainCameraIndex();
            nowCameraIndex = mainCameraIndex;
            // Debug.Log(additionalCameras.Length);
            // cameras.Add(Camera.main);
            // cameras.AddRange(FindObjectsOfType<Camera>());            
        }


        void Update()
        {
            if (promptText.enabled)
            {
                CheckButton();
            }
        }

        private int FindMainCameraIndex()
        {
            for (int i = 0; i < additionalCameras.Length; i++)
                {
                    if (additionalCameras[i] == mainCamera)
                    {
                        return i;
                    }
                }
            return -1;
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
                    additionalCameras[mainCameraIndex].enabled = true;
                    nowCameraIndex = mainCameraIndex;                    
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
                    Debug.Log("Previous Camera");
                    Debug.Log(nowCameraIndex);
                    
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    NextCameraActivation();
                    Debug.Log("Next Camera");
                    Debug.Log(nowCameraIndex);
                }
            }
        } 

        private void NextCameraActivation()
        {
            currentCameraIndex++;
            if (currentCameraIndex==mainCameraIndex)
                {
                    currentCameraIndex++;
                }
            if (currentCameraIndex >= additionalCameras.Length)
                {
                    currentCameraIndex = 0;
                }
            if (currentCameraIndex==mainCameraIndex)
                {
                    currentCameraIndex++;
                }
            additionalCameras[nowCameraIndex].enabled = false;
            additionalCameras[currentCameraIndex].enabled = true;
            nowCameraIndex = currentCameraIndex;
        }

        private void PreviousCameraActivation()
        {
            currentCameraIndex--;
            if (currentCameraIndex==mainCameraIndex)
                {
                    currentCameraIndex--;
                }
            if (currentCameraIndex < 0)
                {
                    currentCameraIndex = additionalCameras.Length - 1;
                }
            if (currentCameraIndex==mainCameraIndex)
                {
                    currentCameraIndex--;
                }
            additionalCameras[nowCameraIndex].enabled = false;
            additionalCameras[currentCameraIndex].enabled = true;
            nowCameraIndex = currentCameraIndex;
        }

        // private void DoAction()
        // {
        //     // Ваше действие при нажатии клавиши "F"
        //     Debug.Log("Выполняется действие при нажатии клавиши F"); 
        //     Debug.Log(isMainCamera);      
                
        //     if (isMainCamera)
        //     {
        //         Debug.Log(isMainCamera);        

        //         mainCamera.enabled = false;
                
        //         watchCamera.enabled = true; 
        //         isMainCamera = false;
        //     }
        //     else
        //     {
        //         mainCamera.enabled = true;
        //         isMainCamera = true;
        //         watchCamera.enabled = false; 
        //     }            
        // }


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
