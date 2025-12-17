using System;
using System.IO;

namespace Arkivverket.Arkade.Core.Tests.UnitTestUtilities;

public class DisposableDirectory : IDisposable
{
    private readonly DirectoryInfo _directory;

    public DisposableDirectory(string directoryPath) : this(new DirectoryInfo(directoryPath))
    {
    }
    
    public DisposableDirectory(DirectoryInfo directory)
    {
        if (directory.Exists)
            throw new ArgumentException($"Directory {directory} already exists");

        directory.Create();

        _directory = directory;
    }

    public DirectoryInfo Get()
    {
        return _directory;
    }

    public void Dispose()
    {
        _directory.Delete(true);
    }
}
