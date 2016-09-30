using System;
using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Tests.Noark5.Structure
{
    public class CheckWellFormedXml : BaseTest
    {
        public CheckWellFormedXml(IArchiveContentReader archiveReader) : base(TestType.Structure, archiveReader)
        {
        }

        protected override void Test(Archive archive)
        {
            var structureDescriptionFileName = archive.GetStructureDescriptionFileName();
            try
            {
                new Common.CheckWellFormedXml().Test(structureDescriptionFileName);
                TestSuccess($"Filen {structureDescriptionFileName} inneholder gyldig xml.");
            }
            catch (Exception e)
            {
                TestError($"Validering av gyldig xml feilet for filen: {structureDescriptionFileName}\n{e.Message}");
            }
        }
    }
}