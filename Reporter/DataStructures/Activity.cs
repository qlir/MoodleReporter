namespace ReportsGenerator.DataStructures
{
    using Newtonsoft.Json;

    public class Activity
    {
        [JsonProperty("grademax")]
        public double GradeMax
        {
            get;
            set;
        }

        [JsonProperty("grademin")]
        public double GradeMin
        {
            get;
            set;
        }

        [JsonProperty("gradepass")]
        public double GradePass
        {
            get;
            set;
        }

        [JsonProperty("grades")]
        public Grade[] Grades
        {
            get;
            set;
        }

        [JsonProperty("activityid")]
        public string Id
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