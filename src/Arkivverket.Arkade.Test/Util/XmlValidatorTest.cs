using Arkivverket.Arkade.Test.Tests.Noark5;
using Arkivverket.Arkade.Util;
using Xunit;
using FluentAssertions;

namespace Arkivverket.Arkade.Test.Util
{
    public class XmlValidatorTest
    {

        private readonly string _addmlXsd = ResourceUtil.ReadResource(ArkadeConstants.AddmlXsdResource);
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

            validationErrorMessages[0].Should().Contain("datasett");
        }

    }
}
