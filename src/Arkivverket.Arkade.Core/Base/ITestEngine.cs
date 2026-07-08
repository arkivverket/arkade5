using Arkivverket.Arkade.Core.Base.Archives;

namespace Arkivverket.Arkade.Core.Base
{
    public interface ITestEngine
    {
        TestSuite RunTestsOnArchive(Archive archive);
    }
}