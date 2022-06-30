using CommandLine;

namespace Arkivverket.Arkade.CLI.Options
{
    internal static class OptionsConfig
    {
        internal static readonly UnParserSettings FormatStyle = new()
        {
            PreferShortName = true, // --alpha -> -a
            GroupSwitches = true, // -a -b -> -ab
        };
    }
}
