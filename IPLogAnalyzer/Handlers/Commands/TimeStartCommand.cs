using IPLogAnalyzer.Models;
using IPLogAnalyzer.Services;

namespace IPLogAnalyzer.Handlers.Commands
{
    internal class TimeStartCommand : CommandHandler
    {
        public override string Name => CommandNames.StartTime;

        public override void Execute(string[] args, ref int i, LogAnalysisParameters arguments)
        {
            if (i + 1 < args.Length)
            {
                arguments.StartTime = ParseService.ValidateTime(args[i], args[i + 1]);
                i++;
            }
            else
            {
                throw new ArgumentException("Отсутствует время начала интервала после параметра --time-start");
            }
        }
    }
}
