using System;

namespace Arkivverket.Arkade.Core
{
    public interface ITestEngine
    {
        TestSuite RunTestsOnArchive(TestSession testSession);
    }
}