using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #31
    /// </summary>
    public class NumberOfDepreciations : CountElementsWithUniqueName
    {
        public NumberOfDepreciations() : base("avskrivning")
        {
        }

        public override string GetName()
        {
            return Noark5Messages.NumberOfDepreciations;
        }

        protected override string GetResultMessage()
        {
            return Noark5Messages.NumberOfDepreciationsMessage;
        }
    }
}
