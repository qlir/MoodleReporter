using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportsGenerator
{
    public class Strings
    {
        internal const string M = "М";
        public const char PatronymicLastLeter = 'ч';
        internal const string TempFolder = "\\temp";
        internal const string DateFormatForFolderName = "yyyy-mm-dd HH.mm.ss";
        internal const string Slash = "/";
        internal const string EmailName = "{0}-{1}.eml"; // 0- is email to; 1 -is subject
        internal const string SavingError = "Ошибка сохранения письма: {0}";
        internal const string SendingError = "Ошибка отправки письма: {0}";
    }
}
