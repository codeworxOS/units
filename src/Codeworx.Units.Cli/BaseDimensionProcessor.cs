using System;

namespace Codeworx.Units.Cli
{
    public abstract class BaseDimensionProcessor
    {
        public Options CurrentOptions { get; }

        public BaseDimensionProcessor(Options options)
        {
            CurrentOptions = options;
        }


        protected void WriteErrorOutput(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"[Error] => {message}");
            Console.ResetColor();
        }

        protected void WriteWarningOutput(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Error.WriteLine($"[Warning] => {message}");
            Console.ResetColor();
        }

        protected void WriteVerboseInfo(string message)
        {
            if (CurrentOptions.Verbose)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }
    }
}
