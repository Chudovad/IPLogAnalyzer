using IPLogAnalyzer.Models;
using IPLogAnalyzer.Services;

namespace IPLogAnalyzer.Handlers.Commands
{
    internal class AddressMaskCommand : CommandHandler
    {
        public override string Name => CommandNames.AddressMask;

        public override void Execute(string[] args, ref int i, LogAnalysisParameters arguments)
        {
            if (i + 1 < args.Length)
            {
                arguments.Mask = ParseService.ValidateIPAddress(args[i], args[i + 1]);
                i++;
            }
            else
            {
                throw new ArgumentException("Отсутствует маска подсети после параметра --address-mask");
            }
        }
    }
}
