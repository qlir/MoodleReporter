namespace ReportsGenerator.DataStructures
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class User : ICurator
    {
        public string Caption
        {
            get;
            set;
        }

        [JsonProperty("email")]
        public string Email
        {
            get;
            set;
        }

        [JsonProperty("firstname")]
        public string FirstName
        {
            get;
            set;
        }

        [JsonProperty("fullname")]
        public string FullName
        {
            get;
            set;
        }

        [JsonProperty("groups")]
        public IEnumerable<Group> Groups
        {
            get;
            set;
        }

        [JsonProperty("id")]
        public string Id
        {
            get;
            set;
        }

        [JsonProperty("institution")]
        public string Institution
        {
            get;
            set;
        }

        public bool IsMan
        {
            get
            {
                var trimmend = FirstName.Trim();
                return trimmend[trimmend.Length - 1] == 'ч';
            }
        }

        public string Direction
        {
            get
            {
                return null;
            }
        }

        public string TemplateName
        {
            get
            {
                return null;
            }
        }

        [JsonProperty("lastname")]
        public string LastName
        {
            get;
            set;
        }

        [JsonProperty("roles")]
        public IEnumerable<Role> Roles
        {
            get;
            set;
        }

        [JsonProperty("username")]
        public string UserName
        {
            get;
            set;
        }

        public class Role
        {
            [JsonProperty("roleid")]
            public int Id
            {
                get;
                set;
            }
        }
    }
}