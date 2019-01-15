using System;
using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Addml;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml
{
    public class FlatFileReaderFactoryTest
    {
        private static Archive CreateArchive(ArchiveType archiveType)
        {
            return new Archive(archiveType, Uuid.Random(), new WorkingDirectory(new DirectoryInfo(@"c:\temp")));
        }

        [Fact(Skip = "flatFile == null wont work!")]
        public void ShouldReturnFlatFileReaderForNoark3()
        {
            var reader = new FlatFileReaderFactory().GetRecordEnumerator(CreateArchive(ArchiveType.Noark3), null);
            reader.GetType().Should().Be(typeof(FixedFileFormatReader));
        }

        [Fact(Skip = "flatFile == null wont work!")]
        public void ShouldReturnFlatFileReaderForFagsystem()
        {
            var reader = new FlatFileReaderFactory().GetRecordEnumerator(CreateArchive(ArchiveType.Fagsystem), null);
            reader.GetType().Should().Be(typeof(FixedFileFormatReader));
        }

        [Fact(Skip = "flatFile == null wont work!")]
        public void ShouldReturnNoark4FileReaderForNoark4()
        {
            var reader = new FlatFileReaderFactory().GetRecordEnumerator(CreateArchive(ArchiveType.Noark4), null);
            reader.GetType().Should().Be(typeof(Noark4FileReader));
        }

        [Fact(Skip = "Initialization of Archive expects addml-file in workingdirectory")]
        public void ShouldThrowExceptionForNoark5()
        {
            Assert.Throws<ArgumentException>(
                () => new FlatFileReaderFactory().GetRecordEnumerator(CreateArchive(ArchiveType.Noark5), null)
                );
        }
    }
}