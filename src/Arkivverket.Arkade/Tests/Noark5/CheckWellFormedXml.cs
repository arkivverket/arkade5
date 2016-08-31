using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class CheckWellFormedXml : BaseTest
    {
        protected override TestResults Test(ArchiveExtraction archive)
        {
            new Common.CheckWellFormedXml().Test(archive.GetContentDescriptionFileName());
            return new TestResults();
        }
    }
}