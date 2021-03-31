using CommandLine;

namespace Arkivverket.Arkade.CLI
{
    public abstract class Options
    {
        [Option('o', "output-directory", HelpText = "Directory to place Arkade output files.", Required = true)]
        public string OutputDirectory { get; set; }

        [Option('l', "language",
            Default = "nb",
            HelpText =
                "Optional. Set language for Arkade output files.\n" +
                "Supported languages:\n" +
                "\t'nb' (Norwegian bokmaal)\n" +
                "\t'en' (British English)")]
        public string OutputLanguage { get; set; }
    }
}
