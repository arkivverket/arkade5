using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Tests.Noark5.Structure
{
    public class CheckWellFormedXml : BaseTest
    {
        public CheckWellFormedXml() : base(TestType.Structure)
        {
        }

        protected override TestResults Test(ArchiveExtraction archive)
        {
            new Common.CheckWellFormedXml().Test(archive.GetStructureDescriptionFileName());
            return new TestResults();
        }
    }
}