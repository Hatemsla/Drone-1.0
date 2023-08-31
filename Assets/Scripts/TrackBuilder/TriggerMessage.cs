using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Drone;
using UnityEngine.Networking;
using System.IO;

namespace Drone.Builder
{
    public class TriggerMassege : InteractiveObject
    {
        public string triggerText;
        [SerializeField] private HelpMessage message;
        public GameObject TriggerObject;
        private Renderer objectRenderer;
        public ColorOption selectedColorOption;
        private MeshRenderer mesh;
        private bool FirstEnter;

        public AudioSource audioSource;
        private string filePath;
        float openTime;
        bool firsTrigger = true;


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

        private void OnTriggerEnter(Collider other)
        {
            if (!FirstEnter || !BuilderManager.Instance.isMove)
            {
                return;
            }
            if (other.GetComponentInParent<DroneController>() is DroneBuilderController drone)
            {
                if (audioSource.clip != null)
                {
                    audioSource.Play();
                }
                if (triggerText.Length > 1)
                {
                    message.ChangeMessageText(triggerText);
                    message.SetActive(true);

                }                
                FirstEnter = false;
                StartCoroutine(closeMessage());
                
            }
        }

        private void IfGameStoped()
        {
            audioSource.Stop();
            FirstEnter = true;        
        }

        IEnumerator closeMessage()
        {

            yield return new WaitForSeconds(10);

            message.SetActive(false);

        }


        private void ShowMassage()
        {

        }

        private void FindMessage()
        {
            message = BuilderManager.Instance.builderUI.helpMessage;
        }

        private void OnEnable()
        {
            BuilderManager.Instance.ObjectChangeSceneEvent += FindMessage;
            BuilderManager.Instance.StopGame += IfGameStoped;
        }
        
        private void OnDisable()
        {
            BuilderManager.Instance.ObjectChangeSceneEvent -= FindMessage;
            BuilderManager.Instance.StopGame -= IfGameStoped;
        }

        private void SetMessageText(string value)
        {
            triggerText = value;
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

        private void LoadAudio(string filePath)
        {
            // StartCoroutine(LoadAudioCoroutine(filePath));
        }

        private IEnumerator LoadAudioCoroutine(string filePath)
        {
            Debug.Log(filePath);
            using var www = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.OGGVORBIS);
            
            yield return www;

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("clip setted");
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = clip;
            }
            else
            {
                Debug.LogError("Error loading audio: " + www.error);
            }
        }

        public void SetSoundFile(int soundIndex)
        {
            sound_index = soundIndex;
            string[] files = Directory.GetFiles(Application.dataPath + "/SoundsSource/");
            filePath = files[soundIndex];
            Debug.Log(filePath);
            StartCoroutine(LoadAudioCoroutine(filePath));
        }

        
    }
}
