using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;

namespace Arkivverket.Arkade.Core.Testing
{
    public interface ITestProvider
    {
        List<INoark5Test> GetContentTests(Archive archive);

        List<IArkadeStructureTest> GetStructureTests();
    }
}