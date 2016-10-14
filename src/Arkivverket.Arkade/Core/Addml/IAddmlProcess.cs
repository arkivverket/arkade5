using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Addml
{
    public interface IAddmlProcess
    {
        List<TestRun> GetTestRuns();
    }
}