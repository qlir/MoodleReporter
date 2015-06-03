namespace ReportsGenerator.DataStructures
{
    using Newtonsoft.Json;

    public class MoodleException
    {
        [JsonProperty("message")]
        public string Message
        {
            get;
            set;
        }
    }
}