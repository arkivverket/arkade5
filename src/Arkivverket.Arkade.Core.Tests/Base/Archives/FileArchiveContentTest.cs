using System;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base.Archives;

public class FileArchiveContentTest
{
    private static readonly FileInfo TestFile = new(
        Path.Combine(Environment.CurrentDirectory, "TestData", "Archives", "Siard", "extraction", "dbptk.siard"));

    private readonly FileArchiveContent _content = new(TestFile);

    [Fact]
    public void RootFile_ShouldBeTheProvidedFile()
    {
        _content.RootFile.Should().Be(TestFile);
    }

    [Fact]
    public void FetchAllTest()
    {
        var contentItems = _content.FetchAll().ToArray();

        contentItems.Should().HaveCount(1);
        contentItems[0].FullPath.Should().Be(_content.RootFile.FullName);
        contentItems[0].RelativePath.Should().Be(_content.RootFile.Name);
        contentItems[0].IsDirectory.Should().BeFalse();
    }
}
