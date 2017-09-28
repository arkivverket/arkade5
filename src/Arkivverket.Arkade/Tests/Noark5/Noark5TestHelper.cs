using Arkivverket.Arkade.Core.Noark5;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public static class Noark5TestHelper
    {
        public static bool IdentifiesJournalPostRegistration(ReadElementEventArgs eventArgs)
        {
            return eventArgs.Path.Matches("registrering") &&
                   eventArgs.Name.Equals("xsi:type") &&
                   eventArgs.Value.Equals("journalpost");
        }
    }
}
