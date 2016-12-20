using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #26
    /// </summary>
    public class NumberOfCaseParts : CountElementsWithUniqueName
    {
        public NumberOfCaseParts() : base("sakspart")
        {
        }

        public override string GetName()
        {
            return Noark5Messages.NumberOfCaseParts;
        }

        protected override string GetResultMessage()
        {
            return Noark5Messages.NumberOfCasePartsMessage;
        }

    }
}
