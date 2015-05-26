using System.Collections.Generic;
using Newtonsoft.Json;

namespace ReportsGenerator.DataStructures
{
    public class User : ICurator
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("username")]
        public string UserName { get; set; }
        
        [JsonProperty("firstname")]
        public string FirstName { get; set; }
        
        [JsonProperty("lastname")]
        public string LastName { get; set; }
        
        [JsonProperty("fullname")]
        public string FullName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
        
        [JsonProperty("institution")]
        public string Institution { get; set; }

        [JsonProperty("groups")]
        public IEnumerable<Group> Groups { get; set; }

        [JsonProperty("roles")]
        public IEnumerable<Role> Roles { get; set; }

        public string Caption { get; set; }

        public bool IsMan
        {
            get
            {
                var trimmend = FirstName.Trim();
                return trimmend[trimmend.Length - 1] == Strings.PatronymicLastLeter;
            }
        }

        public class Role
        {
            [JsonProperty("roleid")]
            public int Id { get; set; }
        }
    }
}
