using Newtonsoft.Json;
using UnityEngine;

namespace Sockets
{
    public enum DroneMode
    {
        AltHold,
        Angle,
        Manual
    }
    
    public class ScratchData
    {
        [JsonProperty("Mode")]
        public string ModeString;
        [JsonIgnore]
        public DroneMode Mode;
        public float Roll;
        public float Pitch;
        public float Yaw;
        public float Althold;
        public float Angle;
        public float Pp;
        public float Pl;
        public float Zp;
        public float Zl;
        
        [JsonConstructor]
        private ScratchData()
        {
        }
        
        public ScratchData(string modeString)
        {
            ModeString = modeString;
            Mode = ParseModeString(modeString);
        }

        private DroneMode ParseModeString(string modeString)
        {
            switch (modeString)
            {
                case "AltHold":
                    return DroneMode.AltHold;
                case "Angle":
                    return DroneMode.Angle;
                case "Manual":
                    return DroneMode.Manual;
                default:
                    Debug.LogError("Unknown mode: " + modeString);
                    return DroneMode.AltHold; // Возвращаем значение по умолчанию или обработку ошибки
            }
        }
    }
}