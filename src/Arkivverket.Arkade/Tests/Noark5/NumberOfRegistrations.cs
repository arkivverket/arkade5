using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfRegistrations : CountElementsWithUniqueName

    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 0); // TODO: Assign correct test number

        public NumberOfRegistrations() : base("registrering")
        {
        }

        public override TestId GetId()
        {
            return _id;
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