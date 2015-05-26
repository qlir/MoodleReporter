using System;

namespace ReportsGenerator
{
    public class ReporterException : Exception
    {
        public ReporterException(string message) : base(message) { }
        public ReporterException(string message, Exception e) : base(message, e) { }
    }
}