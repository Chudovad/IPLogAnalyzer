using IPLogAnalyzer.Models;

namespace IPLogAnalyzer.Handlers
{
    internal abstract class CommandHandler
    {
        public abstract string Name { get; }

        public abstract void Execute(string[] args, ref int i, LogAnalysisParameters arguments);
    }
}
