using Arkivverket.Arkade.Test.Tests.Noark5;
using Arkivverket.Arkade.Util;
using Xunit;
using FluentAssertions;

namespace Arkivverket.Arkade.Test.Util
{
    public class XmlUtilTest
    {

        private string addmlXsd = ResourceUtil.ReadResource(ArkadeConstants.AddmlXsdResource);
        private string addml = TestUtil.ReadFromFileInTestDataDir("noark3\\addml.xml");

        [Fact]
        public void ShouldNotCreateErrorsIfIfXmlValidateAgainstSchema()
        {
            var validationErrorMessages = new XmlUtil().Validate(addml, addmlXsd);

            validationErrorMessages.Count.Should().Be(0);
        }

        [Fact]
        public void ShouldThrowExceptionIfXmlDoesNotValidateAgainstSchema()
        {
            var invalidAddml = addml.Replace("dataset", "datasett");

            var validationErrorMessages = new XmlUtil().Validate(invalidAddml, addmlXsd);

            validationErrorMessages[0].Should().Contain("datasett");
        }

    }
}
