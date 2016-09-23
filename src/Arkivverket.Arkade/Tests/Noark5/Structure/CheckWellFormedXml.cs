using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Tests.Noark5.Structure
{
    public class CheckWellFormedXml : BaseTest
    {
        public CheckWellFormedXml(IArchiveContentReader archiveReader) : base(TestType.Structure, archiveReader)
        {
        }
        public override string GetName()
        {
            return this.GetType().Name;
        }

        protected override void Test(Archive archive)
        {
            new Common.CheckWellFormedXml().Test(archive.GetStructureDescriptionFileName());
        }
    }
}