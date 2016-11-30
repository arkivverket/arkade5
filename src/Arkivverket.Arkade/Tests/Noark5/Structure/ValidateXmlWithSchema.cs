using System;
using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5.Structure
{
    /// <summary>
    ///     Validates that the XML is valid with regards to the XML schema. In this case the ADDML schema.
    /// </summary>
    public class ValidateXmlWithSchema : BaseNoark5Test
    {
        public ValidateXmlWithSchema(IArchiveContentReader archiveReader) : base(TestType.Structure, archiveReader)
        {
        }

        protected override void Test(Archive archive)
        {
            Stream addmlXsd = ResourceUtil.GetResourceAsStream(ArkadeConstants.AddmlXsdResource);

            try
            {
                XmlUtil.Validate(ArchiveReader.GetStructureContentAsStream(archive), addmlXsd);

                TestSuccess(new Location(archive.GetStructureDescriptionFileName()), $"Filen {archive.GetStructureDescriptionFileName()} er validert i henhold ADDML XML-skjema.");
            }
            catch (Exception e)
            {
                TestError(new Location(archive.GetStructureDescriptionFileName()), $"Filen {archive.GetStructureDescriptionFileName()} er ikke gyldig i henhold til ADDML XML-skjema:\n{e.Message}");
            }
        }

        public override void OnReadStartElementEvent(object sender, ReadElementEventArgs e)
        {
        }

    }
}