using IPLogAnalyzer.Models;
using IPLogAnalyzer.Services;

namespace IPLogAnalyzer.Handlers.Commands
{
    internal class AddressStartCommand : CommandHandler
    {
        public override string Name => CommandNames.StartAddress;

        public override void Execute(string[] args, ref int i, LogAnalysisParameters arguments)
        {
            if (i + 1 < args.Length)
            {
                arguments.StartAddress = ParseService.ValidateIPAddress(args[i], args[i + 1]);
                i++;
            }
            else
            {
                throw new ArgumentException("Отсутствует IP-адрес после параметра --address-start");
            }
        }
    }
}
