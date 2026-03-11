using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Xunit;
using Arkivverket.Arkade.Core.Tests.UnitTestUtilities;

[assembly: AssemblyFixture(typeof(TestSessionLifeTimeFilesFixture))]

namespace Arkivverket.Arkade.Core.Tests.UnitTestUtilities;

public class TestSessionLifeTimeFilesFixture : IDisposable
{
    private readonly DirectoryInfo _tmpDirectory;

    public TestSessionLifeTimeFilesFixture()
    {
        _tmpDirectory = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".tmp"));

        if (_tmpDirectory.Exists)
            Dispose();

        _tmpDirectory.Create();
    }

    /// <summary>
    /// Creates a directory to serve as a sandbox for tests requiring isolation of temporary files.
    /// The directory name is constructed from a random GUID.
    /// The directory's lifetime is limited to the duration of a test session's execution of tests within this assembly.
    /// </summary>
    /// <returns>A <see cref="DirectoryInfo"/> representing the newly created isolated directory.</returns>
    public DirectoryInfo CreateIsolatedDirectory() => _tmpDirectory.CreateSubdirectory(Guid.NewGuid().ToString());

    /// <summary>
    /// Creates a directory to serve as a sandbox for tests requiring isolation of temporary files.
    /// The directory name is constructed from the type parameter name and the calling member name.
    /// The directory's lifetime is limited to the duration of a test session's execution of tests within this assembly.
    /// </summary>
    /// <typeparam name="T">The type used to generate part of the directory name.</typeparam>
    /// <param name="callerMemberName">The name of the calling method or property (direct caller or overridden).</param>
    /// <returns>A <see cref="DirectoryInfo"/> representing the newly created isolated directory.</returns>
    public DirectoryInfo CreateIsolatedDirectory<T>([CallerMemberName] string callerMemberName = null)
    {
        string assemblyRelativeClassFullName = typeof(T).FullName?[(typeof(T).Assembly.GetName().Name!.Length + 1)..];
        
        var isolatedDirectoryName = $"{assemblyRelativeClassFullName}.{callerMemberName}";
        
        var isolatedDirectory = new DirectoryInfo(Path.Combine(_tmpDirectory.FullName, isolatedDirectoryName));

        if (isolatedDirectory.Exists)
            throw new InvalidOperationException($"Directory '{isolatedDirectory.FullName}' already exists.");

        isolatedDirectory.Create();
        return isolatedDirectory;
    }

    public void Dispose()
    {
        var tries = 3;
        while (_tmpDirectory.Exists && tries-- > 0)
        {
            try
            {
                _tmpDirectory.Delete(true);
                return;
            }
            catch (IOException exception)
            {
                Console.WriteLine($@"Having trouble deleting '{_tmpDirectory}': {exception.Message}. Trying again ...");
                System.Threading.Thread.Sleep(100);
            }
        }

        Console.WriteLine($@"Failed to delete directory '{_tmpDirectory}'");
    }
}
