using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfFolders : CountElementsWithUniqueName
    {
        public NumberOfFolders() : base("mappe")
        {
        }

        public override string GetName()
        {
            return Noark5Messages.NumberOfFolders;
        }

        protected override string GetResultMessage()
        {
            return Noark5Messages.NumberOfFoldersMessage;
        }
    }
}