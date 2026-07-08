using Arkivverket.Arkade.Core.Base.Archives;

namespace Arkivverket.Arkade.Core.Base
{
    public interface IArkadeStructureTest : IArkadeTest
    {
        void Test(Noark5Archive archive);
    }
}
