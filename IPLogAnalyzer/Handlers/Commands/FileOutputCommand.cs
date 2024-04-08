using IPLogAnalyzer.Models;

namespace IPLogAnalyzer.Handlers.Commands
{
    internal class FileOutputCommand : CommandHandler
    {
        public override string Name => CommandNames.FileOutput;

        public override void Execute(string[] args, ref int i, LogAnalysisParameters arguments)
        {
            if (i + 1 < args.Length)
            {
                arguments.OutputFilePath = args[i + 1];
                i++;
            }
            else
            {
                throw new ArgumentException("Отсутствует путь к файлу вывода после параметра --file-output");
            }
        }
    }
}
