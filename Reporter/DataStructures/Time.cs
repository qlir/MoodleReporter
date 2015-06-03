namespace ReportsGenerator.DataStructures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class Time
    {
        public Time(JObject jo)
        {
            Name = jo["name"].ToObject<string>();
            var r = from v in jo["modules"] select v["id"].ToObject<int>();
            // ActivitiesIds = r.ToArray<string>();
        }

        public string[] ActivitiesIds
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }
    }
}