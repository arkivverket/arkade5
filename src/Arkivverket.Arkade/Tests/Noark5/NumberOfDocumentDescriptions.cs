using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #18
    /// </summary>
    public class NumberOfDocumentDescriptions : CountElementsWithUniqueName
    {
        public NumberOfDocumentDescriptions() : base("dokumentbeskrivelse")
        {
        }

        public override string GetName()
        {
            return Noark5Messages.NumberOfDocumentDescriptions;
        }

        protected override string GetResultMessage()
        {
            return Noark5Messages.NumberOfDocumentDescriptionsMessage;
        }
    }
}
