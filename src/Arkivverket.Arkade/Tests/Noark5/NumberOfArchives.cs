using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfArchives : CountElementsWithUniqueName
    {
        public NumberOfArchives() : base("arkiv")
        {
        }

        public override string GetName()
        {
            return Noark5Messages.NumberOfArchives;
        }

        public override TestRun GetTestRun()
        {
            return GetTestRun(Noark5Messages.NumberOfArchivesMessage);
        }
    }
}