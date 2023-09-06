using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

namespace Drone.Builder
{
    public class TriggerMessage : InteractiveObject
    {
        public string triggerText;
        [SerializeField] private string soundName;
        [SerializeField] private HelpMessage message;
        [SerializeField] private ColorOption selectedColorOption;
        [SerializeField] private MeshRenderer mesh;
        private bool _firstEnter;

        [SerializeField] private AudioSource voiceover;
        private string _filePath;
        
        private void Start()
        {
            _firstEnter = true;
            SetColor(GetColorFromOption(selectedColorOption));
        }
        
        private void OnEnable()
        {
            BuilderManager.Instance.ObjectChangeSceneEvent += FindMessage;
            BuilderManager.Instance.StartGame += IsGameStart;
            BuilderManager.Instance.StopGame += IsGameStop;
        }

        private void IsGameStart()
        {
            mesh.enabled = false;
        }

        private void OnDisable()
        {
            BuilderManager.Instance.ObjectChangeSceneEvent -= FindMessage;
            BuilderManager.Instance.StartGame -= IsGameStart;
            BuilderManager.Instance.StopGame -= IsGameStop;
        }

        private void SetColor(Color color)
        {   
            var newColor = new Color(color.r, color.g, color.b, 0.35f);
            mesh.material.SetColor("_Color", newColor);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_firstEnter || !BuilderManager.Instance.isMove) return;
            
            if (other.GetComponentInParent<DroneController>())
            {
                if (voiceover.clip != null)
                {
                    voiceover.Play();
                }
                if (triggerText.Length > 1)
                {
                    message.ChangeMessageText(triggerText);
                    message.SetActive(true);
                }                
                _firstEnter = false;
                StartCoroutine(CloseMessage());
            }
        }

        private void IsGameStop()
        {
            voiceover.Stop();
            mesh.enabled = true;
            _firstEnter = true;        
        }

        private IEnumerator CloseMessage()
        {
            yield return new WaitForSeconds(triggerText.Split().Length * 1.1f);
            message.SetActive(false);
        }

        private void FindMessage()
        {
            message = BuilderManager.Instance.builderUI.helpMessage;
        }
        
        private IEnumerator LoadAudioCoroutine(string filePath)
        {
            using var www = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.MPEG);
            
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                var clip = DownloadHandlerAudioClip.GetContent(www);
                voiceover.clip = clip;

                Debug.Log("Аудиоклип установлен.");
            }
            else
            {
                Debug.LogError("Ошибка при загрузке аудио: " + www.error);
            }
        }

        public string GetSound() => soundName;
        
        public void SetSound(string newSound)
        {
            soundName = newSound;
            var soundPath = Application.dataPath + "/SoundsSource/" + soundName + ".mp3";
            if (File.Exists(soundPath))
            {
                StartCoroutine(LoadAudioCoroutine(soundPath));
            }
            else
            {
                Debug.LogWarning("Файл не найден");
            }
            // var soundSourcePath = Application.dataPath + "/SoundsSource/";
            // var files = Directory.GetFiles(soundSourcePath, "*.mp3");
            // if (soundIndex >= 0 && soundIndex < files.Length)
            // {
            //     var filePath = files[soundIndex];
            //     StartCoroutine(LoadAudioCoroutine(filePath));
            // }
            // else
            // {
            //     Debug.LogError("Некорректный индекс звукового файла.");
            // }
        }
        
        public override void SetActive(bool active)
        {
            isActive = active;
        }

        public override void SetColorIndex(int value)
        {
            colorIndex = value;
            selectedColorOption = (ColorOption)value;
            SetColor(GetColorFromOption(selectedColorOption));
        }
    }
}
