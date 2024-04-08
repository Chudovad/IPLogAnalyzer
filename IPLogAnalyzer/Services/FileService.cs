using System.Globalization;
using System.Net;

namespace IPLogAnalyzer.Services
{
    public class FileService
    {
        public static List<(IPAddress Address, DateTime Timestamp)> ReadLogFile(string filePath)
        {
            var logEntries = new List<(IPAddress Address, DateTime Timestamp)>();

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Файл логов не найден", filePath);
            }

            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                int colonIndex = line.IndexOf(':');

                if (colonIndex != -1)
                {
                    IPAddress address;
                    DateTime timestamp;

                    if (IPAddress.TryParse(line.Substring(0, colonIndex), out address))
                    {
                        if (DateTime.TryParseExact(line.Substring(colonIndex + 1).Trim(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out timestamp))
                        {
                            logEntries.Add((address, timestamp));
                        }
                        else
                        {
                            Console.WriteLine($"Не удалось разобрать время в строке: {line}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Не удалось разобрать IP-адрес в строке: {line}");
                    }
                }
                else
                {
                    Console.WriteLine($"Некорректный формат строки в файле логов: {line}");
                }
            }

            return logEntries;
        }

        public static void WriteResultToFile(string filePath, Dictionary<IPAddress, int> ipAddressCounts)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var pair in ipAddressCounts)
                {
                    writer.WriteLine($"{pair.Key} - {pair.Value}");
                }
            }
        }
    }
}
