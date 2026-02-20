using System;
using System.IO;

namespace Arkivverket.Arkade.Core.Tests.UnitTestUtilities;

public static class TestData
{
    public static string DirectoryPath(params string[] subPathSegments) => PrependTestDataPath(subPathSegments);
    public static DirectoryInfo Directory(params string[] subPathSegments) => new(DirectoryPath(subPathSegments));

    public static string FilePath(params string[] subPathSegments) => PrependTestDataPath(subPathSegments);
    public static FileInfo File(params string[] subPathSegments) => new(FilePath(subPathSegments));

    private static string PrependTestDataPath(string[] subPathSegments) =>
        Path.Combine([AppDomain.CurrentDomain.BaseDirectory, "TestData", .. subPathSegments]);
}
