namespace ReportsGenerator.DataStructures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    public class Grade
    {
        [JsonProperty("userid")]
        public string UserId
        {
            get;
            set;
        }

        [JsonProperty("grade")]
        public double? Value
        {
            get;
            set;
        }
    }
}