using CsvHelper.Configuration;

namespace ReportsGenerator.DataStructures
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Newtonsoft.Json;

    public class Curator : ICurator, INotifyPropertyChanged
    {
        private string _gender;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Caption
        {
            get;
            set;
        }

        [JsonProperty("city")]
        public string City
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

        [JsonProperty("firstName")]
        public string FirstName
        {
            get;
            set;
        }

        [JsonProperty("gender")]
        public string Gender
        {
            get
            {
                return _gender;
            }
            set
            {
                _gender = value;
                OnPropertyChanged();
            }
        }

        /// public static readonly Curator AutoCurator = new Curator() { FullName = "Авто" };
        [JsonProperty("Institution")]
        public string Institution
        {
            get;
            set;
        }

        public bool IsMan
        {
            get
            {
                if (Gender != null)
                {
                    return Gender == "M";
                }
                if (string.IsNullOrEmpty(FirstName))
                {
                    return false;
                }
                var s = FirstName.Trim().Split(' ');
                if (s.Length <= 0)
                {
                    return true;
                }
                return (s.Length > 1 ? s[1] : s[0])[s.Length - 1] == 'ч';
            }
        }

        public string Direction { get; set; }

        [JsonProperty("lastName")]
        public string LastName
        {
            get;
            set;
        }

        public void TryParseGender(string name)
        {
            var patronymicIndex = 1;
            var s = name.Trim().Split(' ');
            var patronymic = s.Length > patronymicIndex ? s[patronymicIndex] : s[0];
            if (string.IsNullOrEmpty(patronymic))
            {
                return;
            }
            Gender = patronymic[patronymic.Length - 1] == 'ч' ? GlobalVariables.Genders[0] : GlobalVariables.Genders[1];
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}