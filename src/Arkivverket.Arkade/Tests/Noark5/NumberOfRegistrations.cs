using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfRegistrations : CountElementsWithUniqueName

    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 16);

        public NumberOfRegistrations() : base("registrering")
        {
        }

        public override TestId GetId()
        {
            return _id;
        }

        public override string GetName()
        {
            return Noark5TestNames.NumberOfRegistrations;
        }

        protected override string GetResultMessage()
        {
            return Noark5Messages.NumberOfRegistrationsMessage;
        }
    }
}