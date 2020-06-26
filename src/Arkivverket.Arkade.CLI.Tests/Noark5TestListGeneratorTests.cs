using Xunit;
using System;
using System.IO;
using FluentAssertions;

namespace Arkivverket.Arkade.CLI.Tests
{
    public class Noark5TestListGeneratorTests
    {
        [Fact]
        public void GenerateTest()
        {
            string workingDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
            string testFilePath = Path.Combine(workingDirectoryPath, "noark5TestList.txt");

            if (File.Exists(testFilePath))
                File.Delete(testFilePath);

            Noark5TestListGenerator.Generate(testFilePath);

            File.Exists(testFilePath).Should().BeTrue();

            var testList = new StringReader(File.ReadAllText(testFilePath));

            testList.ReadLine().Should().Be("# NOARK5-TESTER SOM SKAL UTFØRES AV ARKADE");

            File.Delete(testFilePath);
        }
    }
}
