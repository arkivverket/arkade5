using System;
using System.IO;

namespace Arkivverket.Arkade.Core.Tests.UnitTestUtilities;

public class DisposableDirectory : IDisposable
{
    private readonly DirectoryInfo _directory;

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
        if (_directory.Exists)
            _directory.Delete(true);
    }
}
