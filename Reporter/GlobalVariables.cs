namespace ReportsGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GlobalVariables
    {
        private static readonly string[] genders = { "М", "Ж" };

        public static string[] Genders
        {
            get
            {
                return genders;
            }
        }
    }
}