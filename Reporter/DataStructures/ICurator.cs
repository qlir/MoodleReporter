namespace ReportsGenerator.DataStructures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ICurator
    {
        string Caption
        {
            get;
            set;
        }

        string Email
        {
            get;
        }

        string FirstName
        {
            get;
        }

        string Institution
        {
            get;
        }

        bool IsMan
        {
            get;
        }

        string Direction { get;}
    }
}