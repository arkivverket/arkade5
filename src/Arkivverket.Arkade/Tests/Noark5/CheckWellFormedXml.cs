using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class CheckWellFormedXml : BaseTest
    {
        public CheckWellFormedXml(IArchiveContentReader archiveReader) : base(TestType.Content, archiveReader)
        {
        }

        protected override void Test(ArchiveExtraction archive)
        {
            new Common.CheckWellFormedXml().Test(archive.GetContentDescriptionFileName());
        }
    }
}