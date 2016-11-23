using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfClasses : CountElementsWithUniqueName
    {
        public NumberOfClasses() : base("klasse")
        {
        }

        public override string GetName()
        {
            return Noark5Messages.NumberOfClasses;
        }

        public override TestRun GetTestRun()
        {
            return GetTestRun(Noark5Messages.NumberOfClassesMessage);
        }
    }
}