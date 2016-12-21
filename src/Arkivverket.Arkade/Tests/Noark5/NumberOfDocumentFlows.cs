using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #32
    /// </summary>
    public class NumberOfDocumentFlows : CountElementsWithUniqueName
    {
        public NumberOfDocumentFlows() : base("dokumentflyt")
        {
        }

        public override string GetName()
        {
            return Noark5Messages.NumberOfDocumentFlows;
        }

        protected override string GetResultMessage()
        {
            return Noark5Messages.NumberOfDocumentFlowsMessage;
        }
    }
}
