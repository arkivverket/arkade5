using System;
using Arkivverket.Arkade.Core.Util;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Util
{
    public class PathUtilTests
    {
        [Fact]
        public void GetSubPathTest()
        {
            PathUtil.GetSubPath("dirA", "/dirA/dirB/dirC/dirD/").Should().Be("dirB/dirC/dirD/");
            PathUtil.GetSubPath("dirB", "/dirA/dirB/dirC/dirD/").Should().Be("dirC/dirD/");
            PathUtil.GetSubPath("dirC", "/dirA/dirB/dirC/dirD/").Should().Be("dirD/");
            PathUtil.GetSubPath("dirC/", "/dirA/dirB/dirC/dirD/").Should().Be("dirD/"); // Slash after cutoff
            PathUtil.GetSubPath("/dirC", "/dirA/dirB/dirC/dirD/").Should().Be("dirD/"); // Slash before cutoff
            PathUtil.GetSubPath("//dirA//", "/dirA///dirB/dirC//dirD/").Should().Be("dirB/dirC//dirD/"); // Multiple slashes
            PathUtil.GetSubPath("dirA//", "/dirA/dirB/dirA//dirC/").Should().Be("dirB/dirA//dirC/"); // SubPath of 1st occurrence of cutoff
            PathUtil.GetSubPath("dirA", "\\dirA/dirB/dirC\\dirD/").Should().Be("dirB/dirC\\dirD/"); // Mixed use of path separators
            PathUtil.GetSubPath("dirA\\dirB\\", "/dirA/dirB/dirC/dirD/").Should().Be("dirC/dirD/"); // Use of different path separators in cutoff and path

            PathUtil.GetSubPath("dirD", "/dirA/dirB/dirC/dirD/").Should().BeNull(); // Last element has no subPath
            PathUtil.GetSubPath("dirX", "/dirA/dirB/dirC/dirD/").Should().BeNull(); // Cutoff not found in path

            PathUtil.GetSubPath(string.Empty, "some/path").Should().BeNull();
            PathUtil.GetSubPath("someDir", string.Empty).Should().BeNull();
            PathUtil.GetSubPath(" ", "some/path").Should().BeNull();
            PathUtil.GetSubPath("someDir", " ").Should().BeNull();
            PathUtil.GetSubPath(null, "some/path").Should().BeNull();

            CallingGetSubPathWith("someDir", null).Should().Throw<ArgumentNullException>();
            CallingGetSubPathWith(null, null).Should().Throw<ArgumentNullException>();
        }

        private static Action CallingGetSubPathWith(string cutoff, string path) => () => PathUtil.GetSubPath(cutoff, path);

        [Fact]
        public void GetChildTest()
        {
            PathUtil.GetChild("dirA", "/dirA/dirB/dirC/dirD/").Should().Be("dirB");
            PathUtil.GetChild("dirB", "/dirA/dirB/dirC/dirD/").Should().Be("dirC");
            PathUtil.GetChild("dirC", "/dirA/dirB/dirC/dirD/").Should().Be("dirD");

            PathUtil.GetChild("dirD", "/dirA/dirB/dirC/dirD/").Should().BeNull();
            PathUtil.GetChild("dirX", "/dirA/dirB/dirC/dirD/").Should().BeNull();

            PathUtil.GetChild(string.Empty, "some/path").Should().BeNull();
            PathUtil.GetChild("someDir", string.Empty).Should().BeNull();
            PathUtil.GetChild(" ", "some/path").Should().BeNull();
            PathUtil.GetChild("someDir", " ").Should().BeNull();
            PathUtil.GetChild(null, "some/path").Should().BeNull();

            CallingGetChildWith("someDir", null).Should().Throw<ArgumentNullException>();
            CallingGetChildWith(null, null).Should().Throw<ArgumentNullException>();
        }

        private static Action CallingGetChildWith(string parent, string path) => () => PathUtil.GetChild(parent, path);

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
