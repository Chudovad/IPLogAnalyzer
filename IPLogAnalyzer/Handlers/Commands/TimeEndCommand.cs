using IPLogAnalyzer.Models;
using IPLogAnalyzer.Services;

namespace IPLogAnalyzer.Handlers.Commands
{
    internal class TimeEndCommand : CommandHandler
    {
        public override string Name => CommandNames.EndTime;

        public override void Execute(string[] args, ref int i, LogAnalysisParameters arguments)
        {
            if (i + 1 < args.Length)
            {
                arguments.EndTime = ParseService.ValidateTime(args[i], args[i + 1]);
                i++;
            }
            else
            {
                throw new ArgumentException("Отсутствует время конца интервала после параметра --time-end");
            }
        }
    }
}
