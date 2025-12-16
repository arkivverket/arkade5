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
    public void GetAllContentsTest()
    {
        FileSystemInfo[] allContents = _content.GetAllContents().ToArray();

        allContents.Should().HaveCount(1);
        allContents[0].Should().Be(_content.RootFile);
    }
}
