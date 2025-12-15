using System.Threading.Tasks;
using Codeworx.Units.Cli;
using CommandLine;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var result = Parser.Default.ParseArguments<Options>(args);
        if (result.Tag == ParserResultType.Parsed && result is Parsed<Options>)
        {
            var options = result.Value;

            var inputParser = new InputParser(options);
            if (!await inputParser.ParseDataAsync())
            {
                return;
            }

            var sharpProcessor = new CSharpDimensionCreator(options, inputParser.Result);
            if (!await sharpProcessor.ProcessAsync())
            {
                return;
            }


            if (!string.IsNullOrWhiteSpace(options.Typescript))
            {
                var tsProcessor = new TypescriptDimensionCreator(options, inputParser.Result);
                if (!await tsProcessor.ProcessAsync())
                {
                    return;
                }
            }
        }
    }
}