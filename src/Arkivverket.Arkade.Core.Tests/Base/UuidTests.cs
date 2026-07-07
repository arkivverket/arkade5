using Arkivverket.Arkade.Core.Base;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base;

public class UuidTests
{
    [Theory]
    [InlineData("UUID:258e3353-cef2-407f-92ac-264ad887527b")] // Conventional DIAS OBJID
    [InlineData("uuid:258e3353-cef2-407f-92ac-264ad887527b")] // Prefix match is case-insensitive
    [InlineData("258e3353-cef2-407f-92ac-264ad887527b")] // Bare value (Arkade's own future write-out)
    [InlineData(" UUID:258e3353-cef2-407f-92ac-264ad887527b ")] // Surrounding whitespace
    [InlineData("UUID:258E3353-CEF2-407F-92AC-264AD887527B")] // Upper-case UUID
    public void TryParseFromMetsObjidAcceptsPrefixedAndBareValues(string objid)
    {
        Uuid.TryParseFromMetsObjid(objid, out Uuid uuid).Should().BeTrue();

        uuid.ToString().Should().Be("258e3353-cef2-407f-92ac-264ad887527b");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("UUID:")]
    [InlineData("UUID:not-a-uuid")]
    [InlineData("not-a-uuid")]
    [InlineData("UUID:UUID:258e3353-cef2-407f-92ac-264ad887527b")]
    public void TryParseFromMetsObjidRejectsValuesWithoutAValidUuid(string objid)
    {
        Uuid.TryParseFromMetsObjid(objid, out Uuid uuid).Should().BeFalse();

        uuid.Should().BeNull();
    }
}
