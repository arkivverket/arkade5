using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #4
    /// </summary>
    public class NumberOfClassificationSystems : CountElementsWithUniqueName
    {
        public NumberOfClassificationSystems() : base("klassifikasjonssystem")
        {
        }

        public override string GetName()
        {
            return Noark5Messages.NumberOfClassificationSystems;
        }

        protected override string GetResultMessage()
        {
            return Noark5Messages.NumberOfClassificationSystemsMessage;
        }
    }
}