using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #5
    /// </summary>
    public class NumberOfClasses : CountElementsWithUniqueName
    {
        public NumberOfClasses() : base("klasse")
        {
        }

        public override string GetName()
        {
            return Noark5Messages.NumberOfClasses;
        }

        protected override string GetResultMessage()
        {
            return Noark5Messages.NumberOfClassesMessage;
        }
    }
}