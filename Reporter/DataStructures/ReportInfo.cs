namespace ReportsGenerator.DataStructures
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Newtonsoft.Json;

    using ReportsGenerator.Annotations;

    public class ReportInfo : INotifyPropertyChanged
    {
        private int _groupID;
        private string _groupName;

        public ReportInfo()
        {
            CourseID = -1;
            GroupID = -1;
            StartDate = DateTime.Now;
            EndDate = DateTime.Now;
            CuratorsGenerationType = CuratorsGenerationTypeEnum.MoodleCurators;
        }

        public enum CuratorsGenerationTypeEnum
        {
            All, MoodleCurators, Custom
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [JsonProperty("courseID")]
        public int CourseID
        {
            get;
            set;
        }

        [JsonProperty("curatorEmail")]
        public string CuratorsEmail
        {
            get;
            set;
        }

        public CuratorsGenerationTypeEnum CuratorsGenerationType
        {
            get;
            set;
        }

        [JsonProperty("endDate")]
        public DateTime EndDate
        {
            get;
            set;
        }

        [JsonProperty("groupID")]
        public int GroupID
        {
            get
            {
                return _groupID;
            }
            set
            {
                _groupID = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("groupName")]
        public string GroupName
        {
            get
            {
                return _groupName;
            }
            set
            {
                _groupName = value;
                OnPropertyChanged();
                OnPropertyChanged("GroupTitle");
            }
        }

        public string GroupTitle
        {
            get
            {
                return GroupName ?? (GroupID < 0 ? "" : GroupID.ToString());
            }
        }

        /*
                [JsonProperty("Institution")]
                public string Institution { get; set; }

                [JsonProperty("CuratorEmail")]
                public string CuratorEmail { get; set; }*/
        [JsonProperty("startDate")]
        public DateTime StartDate
        {
            get;
            set;
        }

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /*
                // override object.Equals
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
                    return String.Format("{0}.{1}.{2}", CourseID.GetHashCode(), (CuratorsEmail ?? String.Empty).GetHashCode(), GroupID.GetHashCode()).GetHashCode();
                }*/
    }
}