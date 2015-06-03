namespace Reporter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class Table
    {
        public string[] Activities;
        public float[] CommonAVG;
        public string Institution;
        public Item[] Items;
        public float[] TableAVG;

        UserDataInfo ToInfo;

        public void Table(UserDataInfo toInfo, Item[] items, string[] activities, float[] commonAvg, float[] tableAvg)
        {
            Items = items;
            Activities = activities;
            CommonAVG = commonAvg;
            TableAVG = tableAvg;
            ToInfo = toInfo;
        }

        public class Item
        {
            public float[] Grades;
            public string UserFirstName;
            public string UserLastName;
        }
    }
}