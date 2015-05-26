using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ReportsGenerator.DataStructures
{
    public class Course
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("shortname")]
        public string ShortName { get; set; }

       /* [JsonProperty("fullname")]
        public string FullName { get; set; }*/
    }
}
