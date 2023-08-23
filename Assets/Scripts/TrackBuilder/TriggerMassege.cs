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

        public AudioSource audioSource;
        private string filePath;
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


        private void OnTriggerEnter(Collider other)
        {
            if (!FirstEnter || !BuilderManager.Instance.isMove)
            {
                return;
            }
            if (other.GetComponentInParent<DroneController>() is DroneBuilderController drone)
            {
                if (text3D.Length > 11)
                {
                    if (text3D.Substring(0, 11) == "AudioPlay: ")
                    { 
                        LoadAudio(text3D.Substring(11));
                        return;
                    }
                }
                message.SetActive(true);
                message.ChangeMessageText(text3D);
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

            yield return new WaitForSeconds(5);

            message.SetActive(false);

        }


        private void ShowMassage()
        {

        }

        private void FindMessage()
        {
            message = FindObjectOfType<BuilderUI>().helpMessage;
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

        private void LoadAudio(string filePath)
        {
            StartCoroutine(LoadAudioCoroutine(filePath));
        }

        private IEnumerator LoadAudioCoroutine(string filePath)
        {
            using (var www = new WWW("file://" + filePath)) // Загрузка аудиофайла
            {
                yield return www;

                if (string.IsNullOrEmpty(www.error))
                {
                    Debug.Log("clip setted");
                    AudioClip clip = www.GetAudioClip();
                    audioSource.clip = clip;
                }
                else
                {
                    Debug.LogError("Error loading audio: " + www.error);
                }
            }
            if (audioSource.clip != null)
            {
                audioSource.Play();
            }
        }
    }
}
