using System;
using System.IO;

namespace Arkivverket.Arkade.Core.Tests.UnitTestUtilities;

public static class TestData
{
    private static readonly string TestDataDirectoryPath =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData");

    public static DirectoryInfo Directory() => new(TestDataDirectoryPath);

    public static DirectoryInfo Directory(string testDataDirectorySubPath) =>
        new(Path.Combine(TestDataDirectoryPath, testDataDirectorySubPath));

    public static FileInfo File(string testDataDirectorySubPath) =>
        new(Path.Combine(TestDataDirectoryPath, testDataDirectorySubPath));
}
