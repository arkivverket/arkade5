using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #29
    /// </summary>
    public class NumberOfPrecedents : CountElementsWithUniqueName
    {
        public NumberOfPrecedents() : base("presedens")
        {
        }

        public override string GetName()
        {
            return Noark5Messages.NumberOfPrecedents;
        }

        protected override string GetResultMessage()
        {
            return Noark5Messages.NumberOfPrecedentsMessage;
        }
    }
}
