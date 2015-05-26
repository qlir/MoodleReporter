using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportsGenerator
{
    public class GlobalVariables
    {
        private readonly static string[] genders = { "М", "Ж" };
        public static string[] Genders { get { return genders; } }
    }
}
