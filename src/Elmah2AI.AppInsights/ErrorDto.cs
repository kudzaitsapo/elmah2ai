using System;

namespace Elmah2AI.AppInsights
{
    public class ErrorDto
    {
        public Guid Id { get; set; }

        public string AppName { get; set; }

        public string HostName { get; set; }

        public string TypeName { get; set; }

        public string Source { get; set; }

        public string Message { get; set; }

        public string User { get; set; }

        public int StatusCode { get; set; }

        public DateTime Time { get; set; }

        public string Xml { get; set; }
    }
}
