using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Tests.Noark5.Structure
{
    public class CheckWellFormedXml : BaseTest
    {
        protected override void Test(ArchiveExtraction archive)
        {
            new Common.CheckWellFormedXml().Test(archive.GetStructureDescriptionFileName());
        }
    }
}