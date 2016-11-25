using System;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Noark5;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class CheckWellFormedContentDescriptionXml : BaseTest
    {
        public CheckWellFormedContentDescriptionXml(IArchiveContentReader archiveReader) : base(TestType.ContentAnalysis, archiveReader)
        {
        }

        protected override void Test(Archive archive)
        {
            var contentDescriptionFileName = archive.GetContentDescriptionFileName();
            try
            {
                new Common.CheckWellFormedXml().Test(contentDescriptionFileName);
                TestSuccess(new Location(contentDescriptionFileName), $"Filen {contentDescriptionFileName} inneholder gyldig xml.");
            }
            catch (Exception e)
            {
                TestError(new Location(contentDescriptionFileName), $"Validering av gyldig xml feilet for filen: {contentDescriptionFileName}\n{e.Message}");
            }

        }

        public override void OnReadStartElementEvent(object sender, ReadElementEventArgs e)
        {
        }
    }
}