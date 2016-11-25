using System;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Noark5;

namespace Arkivverket.Arkade.Tests.Noark5.Structure
{
    public class CheckWellFormedArchiveStructureXml : BaseTest
    {
        public CheckWellFormedArchiveStructureXml(IArchiveContentReader archiveReader) : base(TestType.Structure, archiveReader)
        {
        }

        protected override void Test(Archive archive)
        {
            var structureDescriptionFileName = archive.GetStructureDescriptionFileName();
            try
            {
                new Common.CheckWellFormedXml().Test(structureDescriptionFileName);
                TestSuccess(new Location(structureDescriptionFileName), $"Filen {structureDescriptionFileName} inneholder gyldig xml.");
            }
            catch (Exception e)
            {
                TestError(new Location(structureDescriptionFileName), $"Validering av gyldig xml feilet for filen: {structureDescriptionFileName}\n{e.Message}");
            }
        }

        public override void OnReadStartElementEvent(object sender, ReadElementEventArgs e)
        {
        }
    }
}