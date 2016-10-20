using System;

namespace Arkivverket.Arkade.Core
{
    public interface ITestEngine
    {
        event EventHandler<TestStartedEventArgs> TestStarted;
        event EventHandler<TestFinishedEventArgs> TestFinished;

        TestSuite RunTestsOnArchive(TestSession testSession);
    }
}