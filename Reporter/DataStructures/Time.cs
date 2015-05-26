using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportsGenerator.DataStructures
{
    public class Time
    {
        public string Name { get; set; }

        public string[] ActivitiesIds { get; set; }

        public Time(JObject jo)
        {
            Name = jo["name"].ToObject<string>();
            var r = from v in jo["modules"] select v["id"].ToObject<int>();
           // ActivitiesIds = r.ToArray<string>();
        }
    }
}
