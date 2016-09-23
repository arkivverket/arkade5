using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Tests
{
    public interface ITest
    {
        TestRun RunTest(Archive archive);
    }
}