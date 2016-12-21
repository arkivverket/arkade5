using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #30
    /// </summary>
    public class NumberOfCorrespondenceParts : CountElementsWithUniqueName
    {
        public NumberOfCorrespondenceParts() : base("korrespondansepart")
        {
        }

        public override string GetName()
        {
            return Noark5Messages.NumberOfCorrespondenceParts;
        }

        protected override string GetResultMessage()
        {
            return Noark5Messages.NumberOfCorrespondencePartsMessage;
        }
    }
}
