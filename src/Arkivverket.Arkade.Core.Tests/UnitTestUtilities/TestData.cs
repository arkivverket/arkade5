using System;
using System.IO;

namespace Arkivverket.Arkade.Core.Tests.UnitTestUtilities;

public static class TestData
{
    public static DirectoryInfo Directory(params string[] subPathSegments) => new(PrependTestDataPath(subPathSegments));

    public static FileInfo File(params string[] subPathSegments) => new(PrependTestDataPath(subPathSegments));

    private static string PrependTestDataPath(string[] subPathSegments) =>
        Path.Combine([AppDomain.CurrentDomain.BaseDirectory, "TestData", .. subPathSegments]);
}
