using System.IO;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Util;
using Arkivverket.Arkade.Core.Util.FileFormatIdentification;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Util
{
    public class FileSystemInfoSizeCalculatorTest
    {
        private readonly IStatusEventHandler _statusEventHandler;
        private long? _totalFileSize;

        public FileSystemInfoSizeCalculatorTest()
        {
            _statusEventHandler = new StatusEventHandler();
            _statusEventHandler.TargetSizeCalculatorFinishedEvent += OnTargetSizeCalculatorFinished;
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void ShouldCalculateCorrectTotalFileSize()
        {
            string pathToFiles =  Path.Combine("TestData", "FileTypes");

            var fileSystemInfoSizeCalculator = new FileSystemInfoSizeCalculator(_statusEventHandler);

            fileSystemInfoSizeCalculator.CalculateSize(FileFormatScanMode.Directory, pathToFiles);

            while (fileSystemInfoSizeCalculator.IsBusy)
            { //wait
            }

            long docxByteSize = 12895L;
            long zipByteSize = 89899L;
            long pdfByteSize = 27182L;
            long pdfA1bByteSize = 34155L;
            long pdfA3aByteSize = 32506L;

            long totalSize = docxByteSize + zipByteSize + pdfByteSize + pdfA1bByteSize + pdfA3aByteSize;
            _totalFileSize.Should().Be(totalSize); // 5 files + where one is a .zip with 4 files inside
        }

        private void OnTargetSizeCalculatorFinished(object o, TargetSizeCalculatorEventArgs e)
        {
            _totalFileSize = e.TargetSize;
        }
    }
}
