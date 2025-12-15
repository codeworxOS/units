using CommandLine;

namespace Codeworx.Units.Cli
{
    public class Options
    {
        [Option('o', "output", Default = "generated", HelpText = "The output directory for the generated interfaces and structs.")]
        public required string OutputDir { get; set; } = string.Empty;

        [Option('i', "input", Required = true, HelpText = "The source *.units.json File.")]
        public required string Input { get; set; }

        [Option('n', "namespace", Default = "Units", HelpText = "The namespace to use for c# Classes")]
        public required string Namespace { get; set; }

        [Option("typescript", HelpText = "The folder to write the typescript files to")]
        public string? Typescript { get; set; }

        [Option(Default = false, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }
    }
}
