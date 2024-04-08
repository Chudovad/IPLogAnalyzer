using IPLogAnalyzer.Handlers;
using IPLogAnalyzer.Handlers.Commands;
using IPLogAnalyzer.Models;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Net;

namespace IPLogAnalyzer.Services
{
    public class ParseService
    {
        public static LogAnalysisParameters ParseCommandLineArgs(string[] args)
        {
            LogAnalysisParameters logAnalysisParameters = new LogAnalysisParameters();

            if (args.Length < 4)
            {
                throw new ArgumentException("Недостаточно аргументов. Использование: --file-log <путь к файлу> --file-output <путь к файлу> [--address-start <IP-адрес>] [--address-mask <маска подсети>] [--time-start <начальное время>] [--time-end <конечное время>]");
            }

            List<CommandHandler> commands = new List<CommandHandler> 
            { 
                new FileLogCommand(),
                new FileOutputCommand(),
                new AddressStartCommand(),
                new AddressMaskCommand(),
                new TimeStartCommand(),
                new TimeEndCommand(),
            };

            for (int i = 0; i < args.Length; i++)
            {
                if (commands.Any(c => c.Name == args[i]))
                {
                    commands.FirstOrDefault(c => c.Name == args[i]).Execute(args, ref i, logAnalysisParameters);
                }
                else
                {
                    throw new ArgumentException("Неизвестный параметр: " + args[i]);
                }
            }

            ValidateParameters(logAnalysisParameters);

            return logAnalysisParameters;
        }

        public static LogAnalysisParameters ParseLogParametersInConfig(LogAnalysisParameters logAnalysisParameters, IConfigurationRoot config)
        {
            logAnalysisParameters.LogFilePath = config[CommandNames.FileLog];
            logAnalysisParameters.OutputFilePath = config[CommandNames.FileOutput];
            logAnalysisParameters.StartAddress = ValidateIPAddress(CommandNames.StartAddress, config[CommandNames.StartAddress]);
            logAnalysisParameters.Mask = ValidateIPAddress(CommandNames.AddressMask, config[CommandNames.AddressMask]);
            logAnalysisParameters.StartTime = ValidateTime(CommandNames.StartTime, config[CommandNames.StartTime]);
            logAnalysisParameters.EndTime = ValidateTime(CommandNames.EndTime, config[CommandNames.EndTime]);

            ValidateParameters(logAnalysisParameters);

            return logAnalysisParameters;
        }

        private static void ValidateParameters(LogAnalysisParameters logAnalysisParameters)
        {
            if (logAnalysisParameters.LogFilePath == null || logAnalysisParameters.OutputFilePath == null)
            {
                throw new ArgumentException("Не указан путь к файлу логов или к файлу вывода");
            }

            if (logAnalysisParameters.Mask != null && logAnalysisParameters.StartAddress == null)
            {
                throw new ArgumentException("Параметр --address-mask может использоваться только с параметром --address-start");
            }
            
            if (logAnalysisParameters.EndTime == DateTime.MinValue)
            {
                logAnalysisParameters.EndTime = DateTime.MaxValue;
            }

            if (logAnalysisParameters.StartTime > logAnalysisParameters.EndTime)
            {
                throw new ArgumentException("Параметр --time-start должен быть больше --time-end");
            }
        }

        public static IPAddress ValidateIPAddress(string parameterName, string parameterValue)
        {
            IPAddress iPAddress = null;
            if (parameterValue != null && !IPAddress.TryParse(parameterValue, out iPAddress))
                throw new FormatException($"Некорректный формат IP-адреса для {parameterName}.");

            return iPAddress;
        }

        public static DateTime ValidateTime(string parameterName, string parameterValue)
        {
            DateTime timestamp = DateTime.MinValue;
            if (parameterValue != null
                && !DateTime.TryParseExact(parameterValue, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out timestamp))
                throw new FormatException($"Некорректный формат времени для {parameterName}.");

            return timestamp;
        }
    }
}
