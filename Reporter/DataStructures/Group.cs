namespace ReportsGenerator.DataStructures
{
    using Newtonsoft.Json;

    public class Group
    {
        [JsonProperty("id")]
        public int Id
        {
            get;
            set;
        }

        [JsonProperty("name")]
        public string Name
        {
            get;
            set;
        }
    }
}