using Newtonsoft.Json;

namespace GameLauncher.Data
{
    public class CustomServer
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }
    }
}
