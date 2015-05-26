using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reporter
{
    class Table {

        UserDataInfo ToInfo;

        public Item[] Items;

        public string[] Activities;
        public float[] CommonAVG;
        public float[] TableAVG;
        public string Institution;

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
            public string UserFirstName;
            public string UserLastName;
            public float[] Grades;
        }

    }
}
