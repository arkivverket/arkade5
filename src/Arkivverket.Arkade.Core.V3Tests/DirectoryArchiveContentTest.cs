using Arkivverket.Arkade.Core.Base;
using FluentAssertions;

namespace Arkivverket.Arkade.Core.V3Tests;

public class DirectoryArchiveContentTest
{
    private readonly DirectoryInfo _testContentDirectory =
        new(Path.Combine(Environment.CurrentDirectory, "TestData", "Archives", "Noark5", "extraction"));
    
    [Fact]
    public void RootDirectoryTest()
    {
        var content = new DirectoryArchiveContent(_testContentDirectory);
        
        content.RootDirectory.FullName.Should().Be(_testContentDirectory.FullName);
    }

    [Fact]
    public void GetFileTest()
    {
        var content = new DirectoryArchiveContent(_testContentDirectory);
    
        content.GetFile("dokumenter/5000000.pdf").Should().NotBeNull();
        content.GetFile("5000000.pdf").Should().BeNull();
        content.GetFile("dokumenter/nonExisting/5000000.pdf").Should().BeNull(); // Works with caught exception 
    }

    [Fact]
    public void GetDirectoryTest()
    {
        var content = new DirectoryArchiveContent(_testContentDirectory);

        content.GetDirectory("dokumenter").Should().NotBeNull();
        content.GetDirectory("nonExisting").Should().BeNull();
        content.GetDirectory("dokumenter/nonExisting").Should().BeNull(); // Why does it work without catching exception? 
    }

    [Fact]
    public void GetAllContentsTest()
    {
        //return RootDirectory.EnumerateFileSystemInfos("*", SearchOption.AllDirectories);
    }
}
