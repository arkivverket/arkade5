using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #1
    /// </summary>
    public class NumberOfArchives : CountElementsWithUniqueName
    {
        public NumberOfArchives() : base("arkiv")
        {
        }

        public override string GetName()
        {
            return Noark5Messages.NumberOfArchives;
        }

        protected override string GetResultMessage()
        {
            return Noark5Messages.NumberOfArchivesMessage;
        }
    }
}