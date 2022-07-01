using System;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace Arkivverket.Arkade.CLI.Options
{
    [Verb("analyse", HelpText = "Performs a specified type of analysis. Run this command followed by '--help' for more detailed info.")]
    public class AnalyseOptions : OutputOptions
    {
        [Option('f', "format-analysis", HelpText = "Recursively performs PRONOM format analysis on all files in a directory.", Required = true)]
        public string FormatCheckTarget { get; set; }

        [Option('F', "format-analysis-filename", HelpText = "Optional. Provide a custom name for format analysis file.")]
        public string FormatAnalysisResultFileName { get; set; }

        [Obsolete]
        [Option('O', "output-filename", HelpText = "Optional. Overrides default output filename. (NB! Obsolete. Use parameter --format-analysis-filename / -F instead)")]
        public string OutputFileName { get; set; }

        [Usage(ApplicationAlias = "arkade")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Perform format analysis on a specified directory",
                    OptionsConfig.FormatStyle,
                    new AnalyseOptions
                    {
                        OutputDirectory = "outputDirectory",
                        FormatCheckTarget = "/path/to/directory",
                        FormatAnalysisResultFileName = "myFormatInfoFile.csv",
                    });
            }
        }
    }
}
