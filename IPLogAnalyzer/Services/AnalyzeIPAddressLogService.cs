using System.Net;

namespace IPLogAnalyzer.Services
{
    public class AnalyzeIPAddressLogService
    {
        public static List<(IPAddress Address, DateTime Timestamp)> FilterLogEntries(List<(IPAddress Address, DateTime Timestamp)> logEntries, IPAddress startAddress, IPAddress mask, DateTime startTime, DateTime endTime)
        {
            var filteredEntries = logEntries.Where(entry =>
                IsAddressInRange(entry.Address, startAddress, mask) &&
                entry.Timestamp >= startTime &&
                entry.Timestamp <= endTime)
                .ToList();
            return filteredEntries;
        }

        static bool IsAddressInRange(IPAddress ipAddress, IPAddress startIPAddress, IPAddress mask)
        {
            var startIpBytes = startIPAddress.GetAddressBytes();
            var ipAddressBytes = ipAddress.GetAddressBytes();
            var maskBytes = mask.GetAddressBytes();

            for (int i = 0; i < maskBytes.Length; i++)
            {
                if ((ipAddressBytes[i] & maskBytes[i]) != (startIpBytes[i] & maskBytes[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static Dictionary<IPAddress, int> CountIPAddresses(List<(IPAddress Address, DateTime Timestamp)> logEntries)
        {
            var ipAddressCounts = logEntries
                .GroupBy(entry => entry.Address)
                .ToDictionary(group => group.Key, group => group.Count());

            return ipAddressCounts;
        }
    }
}
