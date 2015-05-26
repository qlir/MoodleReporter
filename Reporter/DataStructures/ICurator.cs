using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportsGenerator.DataStructures
{
    public interface ICurator
    {
        string Institution { get; }
        string Email { get; }
        string FullName { get; }
        string Caption { get; set; }
        bool IsMan { get;}
    }
}
