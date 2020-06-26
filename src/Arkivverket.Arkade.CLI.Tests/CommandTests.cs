using System.Collections.Generic;
using System.Linq;
using CommandLine;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.CLI.Tests
{
    public class CommandTests
    {
        [Fact]
        public void CommonInputErrorIsHandled()
        {
            var argsWithExpectedError = new List<(string, ErrorType)>
            {
                // BadVerbSelectedError
                ("-a archive", ErrorType.BadVerbSelectedError),
                ("wrong -a archive", ErrorType.BadVerbSelectedError),

                // MissingRequiredOptionError
                ("process -t type -m metadata -p process-dir -o output-dir", ErrorType.MissingRequiredOptionError),
                ("process -a archive -m metadata -p process-dir -o output-dir", ErrorType.MissingRequiredOptionError),
                ("process -a archive -t type -p process-dir -o output-dir", ErrorType.MissingRequiredOptionError),
                ("process -a archive -t type -m metadata -o output-dir", ErrorType.MissingRequiredOptionError),
                ("process -a archive -t type -m metadata -p process-dir", ErrorType.MissingRequiredOptionError),
                ("pack -t type -m metadata -p process-dir -o output-dir", ErrorType.MissingRequiredOptionError),
                ("pack -a archive -m metadata -p process-dir -o output-dir", ErrorType.MissingRequiredOptionError),
                ("pack -a archive -t type -p process-dir -o output-dir", ErrorType.MissingRequiredOptionError),
                ("pack -a archive -t type -m metadata -o output-dir", ErrorType.MissingRequiredOptionError),
                ("pack -a archive -t type -m metadata -p process-dir", ErrorType.MissingRequiredOptionError),
                ("test -t type -p process-dir -o output-dir", ErrorType.MissingRequiredOptionError),
                ("test -a archive -p process-dir -o output-dir", ErrorType.MissingRequiredOptionError),
                ("test -a archive -t type -o output-dir", ErrorType.MissingRequiredOptionError),
                ("test -a archive -t type -p process-dir", ErrorType.MissingRequiredOptionError),
                ("generate -m", ErrorType.MissingRequiredOptionError),
                ("generate -l", ErrorType.MissingRequiredOptionError),

                //MissingGroupOptionError
                ("generate -p process-dir", ErrorType.MissingGroupOptionError),

                // MissingValueOptionError
                ("process --archive", ErrorType.MissingValueOptionError),
                ("process --type", ErrorType.MissingValueOptionError),
                ("process --metadata-file", ErrorType.MissingValueOptionError),
                ("process --processing-area", ErrorType.MissingValueOptionError),
                ("process --output-directory", ErrorType.MissingValueOptionError),
                ("process --information-package-type", ErrorType.MissingValueOptionError),
                ("pack --archive", ErrorType.MissingValueOptionError),
                ("pack --type", ErrorType.MissingValueOptionError),
                ("pack --metadata-file", ErrorType.MissingValueOptionError),
                ("pack --processing-area", ErrorType.MissingValueOptionError),
                ("pack --output-directory", ErrorType.MissingValueOptionError),
                ("pack --information-package-type", ErrorType.MissingValueOptionError),
                ("test --archive", ErrorType.MissingValueOptionError),
                ("test --type", ErrorType.MissingValueOptionError),
                ("test --processing-area", ErrorType.MissingValueOptionError),
                ("test --output-directory", ErrorType.MissingValueOptionError),
                ("generate --processing-area", ErrorType.MissingValueOptionError),

                // UnknownOptionError
                ("process --metadata-example", ErrorType.UnknownOptionError),
                ("pack --metadata-example", ErrorType.UnknownOptionError),
                ("test --metadata-example", ErrorType.UnknownOptionError),
                ("test --metadata-file", ErrorType.UnknownOptionError),
                ("test --information-package-type", ErrorType.UnknownOptionError),
                ("generate --archive", ErrorType.UnknownOptionError),
                ("generate --type", ErrorType.UnknownOptionError),
                ("generate --metadata-file", ErrorType.UnknownOptionError),
                ("generate --output-directory", ErrorType.UnknownOptionError),
                ("generate --information-package-type", ErrorType.UnknownOptionError),
                ("generate --archive", ErrorType.UnknownOptionError),

                // RepeatedOptionError
                ("process -a archive -a archive", ErrorType.RepeatedOptionError),
                ("pack -a archive -a archive", ErrorType.RepeatedOptionError),
                ("test -a archive -a archive", ErrorType.RepeatedOptionError),
                ("generate -m metadata -m metadata", ErrorType.RepeatedOptionError),
            };

            foreach ((string command, ErrorType expectedError) in argsWithExpectedError)
            {
                IEnumerable<ErrorType> parseErrors = null;

                Program.ParseArguments(command.Split(' ')).WithNotParsed(errors =>
                {
                    parseErrors = errors.Select(e => e.Tag);
                });

                parseErrors.Should().Contain(expectedError);
            }
        }
    }
}
