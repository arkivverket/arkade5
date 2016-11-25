using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #4
    /// </summary>
    public class NumberOfClassificationSystems : CountElementsWithUniqueName
    {
        public NumberOfClassificationSystems() : base("klassifikasjonssystem")
        {
        }

        public override string GetName()
        {
            return Noark5Messages.NumberOfClassificationSystems;
        }

        public override TestRun GetTestRun()
        {
            return GetTestRun(Noark5Messages.NumberOfClassificationSystemsMessage);
        }
    }
}