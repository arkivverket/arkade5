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
    public void FetchAllTest()
    {
        var contentItems = _content.FetchAll().ToArray();

        contentItems.Should().Contain(p =>
            p.FullPath == Path.Combine(TestDirectory.FullName, "addml.xsd") &&
            p.RelativePath == "addml.xsd" &&
            p.IsDirectory == false);

        contentItems.Should().Contain(p =>
            p.FullPath == Path.Combine(TestDirectory.FullName, "dokumenter", "5000000.pdf") &&
            p.RelativePath == "dokumenter/5000000.pdf" &&
            p.IsDirectory == false);

        contentItems.Should().Contain(p =>
            p.FullPath == Path.Combine(TestDirectory.FullName, "dokumenter") &&
            p.RelativePath == "dokumenter" &&
            p.IsDirectory == true);
    }
}
