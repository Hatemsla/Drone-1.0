using System.Collections;
using DroneFootball;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Sockets
{
    public class ScratchClient : MonoBehaviour
    {
        private const string URL = "http://127.0.0.1:5000/droneball/game/get_info/";
        private const string HeightDistURL = "http://127.0.0.1:5000/droneball/game/height_dist_post/{0}/{1}/";
        [SerializeField] private float requestInterval = 0.02f;

        private void Start()
        {
            StartCoroutine(GetData());
        }

        private IEnumerator GetData()
        {
            while (true)
            {
                var www = UnityWebRequest.Get(URL);

                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error: " + www.error);
                }
                else
                {
                    var json = www.downloadHandler.text;

                    var droneData = JsonConvert.DeserializeObject<ScratchData>(json);

                    Debug.Log($"Mode: {droneData.Mode} Roll: {droneData.Roll} Pitch: {droneData.Pitch} Yaw: {droneData.Yaw}");
                    GameManager.Instance.GetScratchData(droneData);
                }

                yield return new WaitForSeconds(requestInterval);
            }
        }
        
        public IEnumerator SendData(float height, float distance)
        {
            var roundedHeight = Mathf.Round(height * 100);
            var roundedDistance = Mathf.Round(distance * 100);
            var url = string.Format(HeightDistURL, roundedHeight, roundedDistance);
            var www = UnityWebRequest.Get(url);

            yield return www.SendWebRequest();
    
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                Debug.Log("Data sent successfully!");
            }
        }
    }
}