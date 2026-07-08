using System;
using System.IO;
using System.Runtime.CompilerServices;
using Xunit;
using Arkivverket.Arkade.CLI.Tests.UnitTestUtilities;

[assembly: AssemblyFixture(typeof(TestSessionLifeTimeFilesFixture))]

namespace Arkivverket.Arkade.CLI.Tests.UnitTestUtilities;

/// <summary>
/// Provides isolated, self-cleaning temporary directories for tests in this assembly.
/// Mirrors the convention used by Arkivverket.Arkade.Core.Tests: a project-local ".tmp"
/// directory under the test output directory, whose lifetime is limited to the assembly's
/// test session. Deletion is retried to tolerate transient file locks (e.g. from external
/// processes) seen on some platforms.
/// </summary>
public class TestSessionLifeTimeFilesFixture : IDisposable
{
    private readonly DirectoryInfo _temporaryDirectory;

    public TestSessionLifeTimeFilesFixture()
    {
        _temporaryDirectory = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".tmp"));

        if (_temporaryDirectory.Exists)
            TryDeleteTemporaryDirectory();

        _temporaryDirectory.Create();
    }

    /// <summary>
    /// Creates an isolated sandbox directory named "&lt;class&gt;.&lt;member&gt;" (from the type
    /// parameter and the calling member). The directory lives for the duration of the assembly's
    /// test session.
    /// </summary>
    /// <typeparam name="T">The type used to generate part of the directory name.</typeparam>
    /// <param name="callerMemberName">The calling method or property name.</param>
    public DirectoryInfo CreateIsolatedDirectory<T>([CallerMemberName] string callerMemberName = null)
    {
        string assemblyRelativeClassFullName = typeof(T).FullName?[(typeof(T).Assembly.GetName().Name!.Length + 1)..];

        var isolatedDirectoryName = $"{assemblyRelativeClassFullName}.{callerMemberName}";

        var isolatedDirectory = new DirectoryInfo(Path.Combine(_temporaryDirectory.FullName, isolatedDirectoryName));

        if (isolatedDirectory.Exists)
            throw new InvalidOperationException($"Directory '{isolatedDirectory.FullName}' already exists.");

        isolatedDirectory.Create();
        return isolatedDirectory;
    }

    public void Dispose()
    {
        if (_temporaryDirectory.Exists)
            TryDeleteTemporaryDirectory();
    }

    private void TryDeleteTemporaryDirectory(int tryNumber = 1)
    {
        try
        {
            _temporaryDirectory.Delete(true);
        }
        catch (IOException exception)
        {
            Console.WriteLine($@"Struggling to delete '{_temporaryDirectory}': {exception.Message}. Trying again ...");
            System.Threading.Thread.Sleep(1000);

            if (tryNumber < 3)
                TryDeleteTemporaryDirectory(tryNumber + 1);
            else
                Console.WriteLine($@"Failed to delete directory '{_temporaryDirectory}'");
        }
    }
}
