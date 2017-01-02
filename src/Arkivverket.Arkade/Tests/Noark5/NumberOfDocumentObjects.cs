using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #21
    /// </summary>
    public class NumberOfDocumentObjects : CountElementsWithUniqueName
    {
        public NumberOfDocumentObjects() : base("dokumentobjekt")
        {
        }

        public override string GetName()
        {
            return Noark5Messages.NumberOfDocumentObjects;
        }

        protected override string GetResultMessage()
        {
            return Noark5Messages.NumberOfDocumentObjectsMessage;
        }
    }
}
