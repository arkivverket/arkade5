using System;
using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Core.Addml
{
    public class FlatFileReaderFactoryTest
    {
        private static Archive CreateArchive(ArchiveType archiveType)
        {
            return new Archive(archiveType, Uuid.Random(), new DirectoryInfo(@"c:\temp"));
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

        [Fact]
        public void ShouldThrowExceptionForNoark5()
        {
            Assert.Throws<ArgumentException>(
                () => new FlatFileReaderFactory().GetRecordEnumerator(CreateArchive(ArchiveType.Noark5), null)
                );
        }
    }
}