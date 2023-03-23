using Newtonsoft.Json;

namespace Sockets
{
    public class MioData
    {
        [JsonProperty("id")]
        public int ID { get; set; }
        [JsonProperty("r")]
        public Armlet RightMio { get; set; }
        [JsonProperty("l")]
        public Armlet LeftMio { get; set; }

        public MioData()
        {
            RightMio = new Armlet();
            LeftMio = new Armlet();
        }
    }
}