using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfFolders : CountElementsWithUniqueName
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 10);

        public NumberOfFolders() : base("mappe")
        {
        }

        public override TestId GetId()
        {
            return _id;
        }

        public override string GetName()
        {
            return Noark5TestNames.NumberOfFolders;
        }

        protected override string GetResultMessage()
        {
            return Noark5Messages.NumberOfFoldersMessage;
        }
    }
}