using System;
using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class CheckWellFormedContentDescriptionXml : BaseTest
    {
        public CheckWellFormedContentDescriptionXml(IArchiveContentReader archiveReader) : base(TestType.Content, archiveReader)
        {
        }

        protected override void Test(Archive archive)
        {
            var contentDescriptionFileName = archive.GetContentDescriptionFileName();
            try
            {
                new Common.CheckWellFormedXml().Test(contentDescriptionFileName);
                TestSuccess($"Filen {contentDescriptionFileName} inneholder gyldig xml.");
            }
            catch (Exception e)
            {
                TestError($"Validering av gyldig xml feilet for filen: {contentDescriptionFileName}\n{e.Message}");
            }

        }
    }
}