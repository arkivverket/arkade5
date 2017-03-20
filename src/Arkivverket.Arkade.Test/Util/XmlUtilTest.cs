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
            var validationErrorMessages = XmlUtil.Validate(addml, addmlXsd);

            validationErrorMessages.Count.Should().Be(0);
        }

        [Fact]
        public void ShouldThrowExceptionIfXmlDoesNotValidateAgainstSchema()
        {
            var invalidAddml = addml.Replace("dataset", "datasett");

            var validationErrorMessages = XmlUtil.Validate(invalidAddml, addmlXsd);

            validationErrorMessages.Should().Contain(m => m.Equals(
                "Elementet addml i navneområdet http://www.arkivverket.no/standarder/addml" +
                " har ugyldig underordnet element datasett i navneområdet http://www.arkivverket.no/standarder/addml." +
                " Forventet liste over mulige elementer: dataset i navneområdet http://www.arkivverket.no/standarder/addml.")
            );
        }

    }
}
