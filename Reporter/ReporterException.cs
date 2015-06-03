namespace ReportsGenerator
{
    using System;

    public class ReporterException : Exception
    {
        public ReporterException(string message)
        : base(message)
        {
        }

        public ReporterException(string message, Exception e)
        : base(message, e)
        {
        }
    }
}