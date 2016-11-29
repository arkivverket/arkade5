using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #2
    /// </summary>
    public class NumberOfArchiveParts : CountElementsWithUniqueName
    {
        public NumberOfArchiveParts() : base("arkivdel")
        {
        }

        public override string GetName()
        {
            return Noark5Messages.NumberOfArchiveParts;
        }

        protected override string GetResultMessage()
        {
            return Noark5Messages.NumberOfArchivePartsMessage;
        }
    }
}