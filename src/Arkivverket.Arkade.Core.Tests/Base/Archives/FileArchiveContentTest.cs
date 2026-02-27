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
        (FileSystemInfo Item, string RelativePath)[] contentItems = _content.GetAll().ToArray();

        contentItems.Should().HaveCount(1);
        contentItems[0].Item.FullName.Should().Be(_content.RootFile.FullName);
        contentItems[0].RelativePath.Should().Be(_content.RootFile.Name);
    }
}
