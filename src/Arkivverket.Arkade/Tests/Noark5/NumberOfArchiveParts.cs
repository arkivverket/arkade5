using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfArchiveParts : CountElementsWithUniqueName
    {
        public NumberOfArchiveParts() : base("arkivdel")
        {
        }

        public override string GetName()
        {
            return Noark5Messages.NumberOfArchiveParts;
        }

        public override TestRun GetTestRun()
        {
            return GetTestRun(Noark5Messages.NumberOfArchivePartsMessage);
        }
    }
}