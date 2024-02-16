using Arkivverket.Arkade.Core.Util;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Util
{
    public class PathUtilTests
    {
        [Fact]
        public void HasIllegalCharactersTest()
        {
            PathUtil.HasIllegalCharacters("illegal/chars/in/path/like/|/").Should().BeTrue();
            PathUtil.HasIllegalCharacters("illegal/chars/in/fileName/like/file<name").Should().BeTrue();

            PathUtil.HasIllegalCharacters("chars/in/path/only/illegal/in/fileNames/like/</").Should().BeFalse();
            PathUtil.HasIllegalCharacters("path/with/mixed\\use/of\\pathSeparators/").Should().BeFalse();
        }
    }
}
