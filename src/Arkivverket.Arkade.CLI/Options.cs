using CommandLine;

namespace Arkivverket.Arkade.CLI
{
    public abstract class Options
    {
        [Option('o', "output-directory", HelpText = "Directory to place Arkade output files.", Required = true)]
        public string OutputDirectory { get; set; }
    }
}
