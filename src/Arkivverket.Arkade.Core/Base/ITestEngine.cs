namespace Arkivverket.Arkade.Core.Base
{
    public interface ITestEngine
    {
        TestSuite RunTestsOnArchive(TestSession testSession);
    }
}