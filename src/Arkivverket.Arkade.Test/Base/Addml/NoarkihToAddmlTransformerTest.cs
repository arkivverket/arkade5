using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.ExternalModels.Addml;
using Arkivverket.Arkade.Test.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Base.Addml
{
    public class NoarkihToAddmlTransformerTest
    {
        [Fact]
        public void ShouldTransformFromNoarkihToAddml()
        {
            string noark4Xml = TestUtil.ReadFromFileInTestDataDir("noark4\\NOARKIH.XML");

            string noark3Xml = NoarkihToAddmlTransformer.Transform(noark4Xml);

            addml addml = AddmlUtil.ReadFromString(noark3Xml);
            addml.Should().NotBeNull();

            addml.dataset[0].flatFiles.flatFile.Length.Should().Be(67);
            addml.dataset[0].flatFiles.flatFileDefinitions.Length.Should().Be(67);
        }
    }
}