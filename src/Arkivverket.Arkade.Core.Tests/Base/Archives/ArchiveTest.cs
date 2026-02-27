using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Archives;
using Arkivverket.Arkade.Core.Tests.UnitTestUtilities;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base.Archives;

public class ArchiveTest
{
    private readonly DirectoryInfo _tmpDirectory = TestData.Directory(".tmp");

    public ArchiveTest()
    {
        if (_tmpDirectory.Exists)
            _tmpDirectory.Delete(true);
    }

    [Fact]
    public void Noark3ArchiveTest()
    {
        using var processingDirectory = new DisposableDirectory(_tmpDirectory);
        var content = new DirectoryArchiveContent(TestData.Directory("Archives", "Noark3", "extraction"));
        var archive = (Noark3Archive)new ArchiveBuilder(content, processingDirectory.Get()).Build<Noark3Archive>();

        List<string> archiveContent = archive.Content.GetAll().Select(i => i.RelativePath).ToList();

        archiveContent.Should().Contain("addml.xml");
        archiveContent.Should().Contain("ARKIV.DAT");
        archiveContent.Should().Contain("DOK.DAT");
        archiveContent.Should().Contain("SAK.DAT");
    }

    [Fact]
    public void Noark4ArchiveTest()
    {
        using var processingDirectory = new DisposableDirectory(_tmpDirectory);
        var content = new DirectoryArchiveContent(TestData.Directory("Archives", "Noark4", "extraction"));
        var archive = (Noark4Archive)new ArchiveBuilder(content, processingDirectory.Get()).Build<Noark4Archive>();

        List<string> archiveContent = archive.Content.GetAll().Select(i => i.RelativePath).ToList();

        archiveContent.Should().Contain("INFO.TXT");
        archiveContent.Should().Contain("NOARKIH.XML");
        archiveContent.Should().Contain("DATA/ARKIV.XML");
        archiveContent.Should().Contain("DOKUMENT/2011/05/17/281.TXT");
    }

    [Fact]
    public void Noark5ArchiveTest()
    {
        using var processingDirectory = new DisposableDirectory(_tmpDirectory);
        var content = new DirectoryArchiveContent(TestData.Directory("Archives", "Noark5", "extraction"));
        var archive = (Noark5Archive)new ArchiveBuilder(content, processingDirectory.Get()).Build<Noark5Archive>();

        List<string> archiveContent = archive.Content.GetAll().Select(i => i.RelativePath).ToList();

        archiveContent.Should().Contain("addml.xsd");
        archiveContent.Should().Contain("arkivstruktur.xml");
        archiveContent.Should().Contain("arkivstruktur.xsd");
        archiveContent.Should().Contain("arkivuttrekk.xml");
        archiveContent.Should().Contain("dokumenter/5000000.pdf");
        archiveContent.Should().Contain("dokumenter/5000001.pdf");
        archiveContent.Should().Contain("metadatakatalog.xsd");
    }

    [Fact]
    public void SiardArchiveTest()
    {
        using var processingDirectory = new DisposableDirectory(_tmpDirectory);
        var content = new FileArchiveContent(TestData.File("Archives", "Siard", "extraction", "dbptk.siard"));
        var archive = (SiardArchive)new ArchiveBuilder(content, processingDirectory.Get()).Build<SiardArchive>();

        List<string> archiveContent = archive.Content.GetAll().Select(i => i.RelativePath).ToList();

        archiveContent.Should().Contain("dbptk.siard");
        archiveContent.Count.Should().Be(1);
    }

    [Fact]
    public void SpecializedSystemArchiveTest()
    {
        using var processingDirectory = new DisposableDirectory(_tmpDirectory);
        var content = new DirectoryArchiveContent(TestData.Directory("Archives", "SpecializedSystem", "extraction"));
        var archive = (SpecializedSystemArchive)new ArchiveBuilder(content, processingDirectory.Get()).Build<SpecializedSystemArchive>();

        List<string> archiveContent = archive.Content.GetAll().Select(i => i.RelativePath).ToList();

        archiveContent.Should().Contain("addml.xml");
        archiveContent.Should().Contain("ut_jeger.dat");
    }
}
