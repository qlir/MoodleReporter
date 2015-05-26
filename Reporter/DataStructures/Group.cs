using Newtonsoft.Json;

namespace ReportsGenerator.DataStructures
{
    public class Group
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
