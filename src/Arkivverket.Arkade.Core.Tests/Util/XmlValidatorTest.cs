using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Tests.Testing.Noark5;
using Arkivverket.Arkade.Core.Util;
using Xunit;
using FluentAssertions;

namespace Arkivverket.Arkade.Core.Tests.Util
{
    public class XmlValidatorTest
    {

        private readonly string _addmlXsd = ResourceUtil.ReadResource(ArkadeConstants.Addml82XsdResource);
        private readonly string _addml = TestUtil.ReadFromFileInTestDataDir("noark3\\addml.xml");

        [Fact]
        public void ShouldNotCreateErrorsIfXmlDoesValidateAgainstSchema()
        {
            var validationErrorMessages = new XmlValidator().Validate(_addml, _addmlXsd);

            validationErrorMessages.Count.Should().Be(0);
        }

        [Fact]
        public void ShouldCreateErrorsIfXmlDoesNotValidateAgainstSchema()
        {
            var invalidAddml = _addml.Replace("dataset", "datasett");

            var validationErrorMessages = new XmlValidator().Validate(invalidAddml, _addmlXsd);

            validationErrorMessages.Keys.First().Should().Contain("datasett");
        }

        [Fact]
        public void ErrorsShouldBeCorrectlyGrouped()
        {
            const string directory = "Noark5\\StructureValidation\\error";
            Stream arkivstrukturXmlWithErrors = TestUtil.ReadFileStreamFromTestDataDir(Path.Combine(directory, ArkadeConstants.ArkivstrukturXmlFileName));
            Stream arkivstrukturXsd = ResourceUtil.GetResourceAsStream(ArkadeConstants.ArkivstrukturXsdResource);
            Stream metadatakatalogXsd = ResourceUtil.GetResourceAsStream(ArkadeConstants.MetadatakatalogXsdResource);

            Dictionary<string, List<long>> validationErrorMessages =
                new XmlValidator().Validate(arkivstrukturXmlWithErrors, new[] { arkivstrukturXsd, metadatakatalogXsd },
                    ArkadeConstants.ArkivstrukturXmlFileName);

            validationErrorMessages.Count.Should().Be(2);

            KeyValuePair<string, List<long>> firstError = validationErrorMessages.First();
            bool firstErrorIsUnique = firstError.Value.Count == 1;

            firstErrorIsUnique.Should().BeTrue();

            KeyValuePair<string, List<long>> secondError = validationErrorMessages.Last();
            bool secondErrorIsNotUniqueAndShouldThereforeBeGrouped = secondError.Value.Count > 1;

            secondErrorIsNotUniqueAndShouldThereforeBeGrouped.Should().BeTrue();
        }

    }
}
