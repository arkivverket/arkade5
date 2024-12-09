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
            string pathToFiles =  Path.Combine("TestData", "DiskUsage");

            var fileSystemInfoSizeCalculator = new FileSystemInfoSizeCalculator(_statusEventHandler);

            fileSystemInfoSizeCalculator.CalculateSize(FileFormatScanMode.Directory, pathToFiles);

            while (fileSystemInfoSizeCalculator.IsBusy)
            { //wait
            }

            long docxByteSize = 12895L;
            long pdfByteSize = 27182L;

            _totalFileSize.Should().Be(docxByteSize + pdfByteSize);
        }

        private void OnTargetSizeCalculatorFinished(object o, TargetSizeCalculatorEventArgs e)
        {
            _totalFileSize = e.TargetSize;
        }
    }
}
