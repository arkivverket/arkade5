using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Tests.Noark5.Structure
{
    public class CheckWellFormedXml : BaseTest
    {
        public CheckWellFormedXml() : base(TestType.Structure)
        {
        }

        protected override void Test(ArchiveExtraction archive)
        {
            new Common.CheckWellFormedXml().Test(archive.GetStructureDescriptionFileName());
        }
    }
}