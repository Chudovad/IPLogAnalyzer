using IPLogAnalyzer.Models;

namespace IPLogAnalyzer.Handlers.Commands
{
    internal class FileLogCommand : CommandHandler
    {
        public override string Name => CommandNames.FileLog;

        public override void Execute(string[] args, ref int i, LogAnalysisParameters parameters)
        {
            if (i + 1 < args.Length)
            {
                parameters.LogFilePath = args[i + 1];
                i++;
            }
            else
            {
                throw new ArgumentException("Отсутствует путь к файлу логов после параметра --file-log");
            }
        }
    }
}
