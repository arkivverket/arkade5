using System;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Tests.UnitTestUtilities;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base.Archives;

public class FileArchiveContentTest
{
    private static readonly FileInfo TestFile = TestData.File("Archives", "Siard", "extraction", "dbptk.siard");

    private readonly FileArchiveContent _content = new(TestFile);

    [Fact]
    public void RootFile_ShouldBeTheProvidedFile()
    {
        _content.RootFile.Should().Be(TestFile);
    }

    [Fact]
    public void GetAllContentsTest()
    {
        FileSystemInfo[] allContents = _content.GetAll().ToArray();

        allContents.Should().HaveCount(1);
        allContents[0].Should().Be(_content.RootFile);
    }

    [Fact]
    public void GetContentRelativePathTest()
    {
        _content.GetContentRelativePath(TestFile).Should().Be(TestFile.Name);

        FileInfo outsideFile = TestData.File("Archives", "Noark3", "extraction", "ARKIV.DAT");
        _content.Invoking(c => c.GetContentRelativePath(outsideFile))
            .Should().Throw<ArgumentException>().WithMessage("The item is not part of the archive content");
        
        DirectoryInfo outsideDirectory = TestData.Directory("Archives", "Noark3", "extraction");
        _content.Invoking(c => c.GetContentRelativePath(outsideDirectory))
            .Should().Throw<ArgumentException>().WithMessage("The item is not part of the archive content");
    }
}
