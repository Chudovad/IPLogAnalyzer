using IPLogAnalyzer.Models;
using IPLogAnalyzer.Services;
using Microsoft.Extensions.Configuration;

namespace IPLogAnalyzer
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                LogAnalysisParameters parameters = new LogAnalysisParameters();

                if (args.Length == 0)
                {
                    var config = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("config.json", optional: true, reloadOnChange: true)
                        .Build();
                    parameters = ParseService.ParseLogParametersInConfig(parameters, config);
                }
                else
                    parameters = ParseService.ParseCommandLineArgs(args);

                var logEntries = FileService.ReadLogFile(parameters.LogFilePath);

                var filteredEntries = AnalyzeIPAddressLogService.FilterLogEntries(logEntries, parameters.StartAddress, parameters.Mask, parameters.StartTime, parameters.EndTime);

                var ipAddressCounts = AnalyzeIPAddressLogService.CountIPAddresses(filteredEntries);

                FileService.WriteResultToFile(parameters.OutputFilePath, ipAddressCounts);

                Console.WriteLine("Анализ завершен. Результаты записаны в файл: " + parameters.OutputFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }

    }
}
