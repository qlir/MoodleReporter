using Newtonsoft.Json;

namespace ReportsGenerator.DataStructures
{
    public class MoodleException
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}