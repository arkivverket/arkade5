using Arkivverket.Arkade.Test.Tests.Noark5;
using Arkivverket.Arkade.Util;
using Xunit;
using FluentAssertions;
using System.Xml.Schema;

namespace Arkivverket.Arkade.Test.Util
{
    public class XmlUtilTest
    {

        private string addmlXsd = ResourceUtil.ReadResource(ArkadeConstants.AddmlXsdResource);
        private string addml = TestUtil.ReadFromFileInTestDataDir("noark3\\addml.xml");

        [Fact]
        public void ShouldNotThrowExceptionIfXmlValidateAgainstSchema()
        {
            XmlUtil.Validate(addml, addmlXsd);
        }

        [Fact]
        public void ShouldThrowExceptionIfXmlDoesNotValidateAgainstSchema()
        {
            var invalidAddml = addml.Replace("dataset", "datasett");

            var exception = Xunit.Assert.Throws<XmlSchemaValidationException>(() => XmlUtil.Validate(invalidAddml, addmlXsd));
            exception.Message.Should().Contain("datasett");
        }

    }
}
