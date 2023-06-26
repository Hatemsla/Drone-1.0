using Newtonsoft.Json;
using UnityEngine;

namespace Sockets
{
    public class ScratchData
    {
        public string Mode;
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
    }
}