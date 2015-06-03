﻿namespace ReportsGenerator.DataStructures
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    using Newtonsoft.Json;

    public class Curator : ICurator, INotifyPropertyChanged
    {
        private string _gender;

        /*        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return GetHashCode().Equals(obj.GetHashCode());
        }

        public override int GetHashCode()
        {
            Course c = null;
            return String.Format("{0}.{1}.{2}.{3}.{4}",
                (Institution ?? String.Empty).GetHashCode(),
                (Email ?? String.Empty).GetHashCode(),
                (FullName ?? String.Empty).GetHashCode(),
                (City ?? String.Empty).GetHashCode(),
                (Gender ?? String.Empty).GetHashCode()).GetHashCode();
        }*/
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
                    return Gender == Strings.M;
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
                return (s.Length > 1 ? s[1] : s[0])[s.Length - 1] == Strings.PatronymicLastLeter;
            }
        }

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