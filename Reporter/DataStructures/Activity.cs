using Newtonsoft.Json;

namespace ReportsGenerator.DataStructures
{
    public class Activity
    {
        [JsonProperty("activityid")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("grademin")]
        public double GradeMin { get; set; }

        [JsonProperty("grademax")]
        public double GradeMax { get; set; }

        [JsonProperty("gradepass")]
        public double GradePass { get; set; }

        [JsonProperty("grades")]
        public Grade[] Grades { get; set; }
    }
}
