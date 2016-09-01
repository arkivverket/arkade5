using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class CheckWellFormedXml : BaseTest
    {
        public CheckWellFormedXml() : base(TestType.Content)
        {
        }

        protected override TestResults Test(ArchiveExtraction archive)
        {
            new Common.CheckWellFormedXml().Test(archive.GetContentDescriptionFileName());
            return new TestResults();
        }
    }
}