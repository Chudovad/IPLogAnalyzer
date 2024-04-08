using System.Net;

namespace IPLogAnalyzer.Models
{
    public class LogAnalysisParameters
    {
        public string LogFilePath { get; set; }
        public string OutputFilePath { get; set; }
        public IPAddress StartAddress { get; set; }
        public IPAddress Mask { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
