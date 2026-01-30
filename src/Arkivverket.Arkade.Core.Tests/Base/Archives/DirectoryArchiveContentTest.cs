using System;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Tests.UnitTestUtilities;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base.Archives;

public class DirectoryArchiveContentTest
{
    private static readonly DirectoryInfo TestDirectory = TestData.Directory(
        Path.Combine("Archives", "Noark5", "extraction"));

    private readonly DirectoryArchiveContent _content = new(TestDirectory);

    [Fact]
    public void RootDirectoryTest()
    {
        _content.RootDirectory.FullName.Should().Be(TestDirectory.FullName);
    }

    [Fact]
    public void GetFileTest()
    {
        _content.GetFile("dokumenter/5000000.pdf").Should().NotBeNull();
        _content.GetFile("5000000.pdf").Should().BeNull();
        _content.GetFile("nonExisting/5000000.pdf").Should().BeNull();
        _content.Invoking(c => c.GetFile(null)).Should().Throw();
    }

    [Fact]
    public void GetDirectoryTest()
    {
        _content.GetDirectory("dokumenter").Should().NotBeNull();
        _content.GetDirectory("nonExisting").Should().BeNull();
        _content.GetDirectory("nonExisting/dokumenter").Should().BeNull();
        _content.Invoking(c => c.GetDirectory(null)).Should().Throw();
    }

    [Fact]
    public void GetAllContentsTest()
    {
        FileSystemInfo[] allContents = _content.GetAll().ToArray();

        allContents.Should().Contain(f => f.Name.Equals("addml.xsd"));
        allContents.Should().Contain(f => f.Name.Equals("5000000.pdf"));
        allContents.Should().Contain(f => f.Name.Equals("dokumenter"));
    }

    [Fact]
    public void GetContentRelativePathTest()
    {
        string fileFullName = Path.Combine(TestDirectory.FullName, "dokumenter", "5000000.pdf");
        FileInfo file = new(fileFullName);
        _content.GetContentRelativePath(file).Should().Be("dokumenter/5000000.pdf");

        string directoryFullName = Path.Combine(TestDirectory.FullName, "dokumenter");
        DirectoryInfo directory = new(directoryFullName);
        _content.GetContentRelativePath(directory).Should().Be("dokumenter");

        FileInfo outsideFile = TestData.File(Path.Combine("Archives", "Noark3", "extraction", "ARKIV.DAT"));
        _content.Invoking(c => c.GetContentRelativePath(outsideFile))
            .Should().Throw<ArgumentException>().WithMessage("The item is not part of the archive content");

        DirectoryInfo outsideDirectory = TestData.Directory(Path.Combine("Archives", "Noark3", "extraction"));
        _content.Invoking(c => c.GetContentRelativePath(outsideDirectory))
            .Should().Throw<ArgumentException>().WithMessage("The item is not part of the archive content");
    }
}
