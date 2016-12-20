using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #27
    /// </summary>
    public class NumberOfComments : CountElementsWithUniqueName
    {
        public NumberOfComments() : base("merknad")
        {
        }

        public override string GetName()
        {
            return Noark5Messages.NumberOfComments;
        }

        protected override string GetResultMessage()
        {
            return Noark5Messages.NumberOfCommentsMessage;
        }

    }
}