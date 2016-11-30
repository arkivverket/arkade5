using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfRegistrations : CountElementsWithUniqueName

    {
        public NumberOfRegistrations() : base("registrering")
        {
        }

        public override string GetName()
        {
            return Noark5Messages.NumberOfRegistrations;
        }

        protected override string GetResultMessage()
        {
            return Noark5Messages.NumberOfRegistrationsMessage;
        }
    }
}