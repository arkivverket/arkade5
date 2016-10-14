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

        [Fact]
        public void ShouldReturnFlatFileReaderForNoark3()
        {
            var reader = new FlatFileReaderFactory().GetReader(CreateArchive(ArchiveType.Noark3));
            reader.GetType().Should().Be(typeof(FlatFileReader));
        }

        [Fact]
        public void ShouldReturnFlatFileReaderForFagsystem()
        {
            var reader = new FlatFileReaderFactory().GetReader(CreateArchive(ArchiveType.Fagsystem));
            reader.GetType().Should().Be(typeof(FlatFileReader));
        }

        [Fact]
        public void ShouldReturnNoark4FileReaderForNoark4()
        {
            var reader = new FlatFileReaderFactory().GetReader(CreateArchive(ArchiveType.Noark4));
            reader.GetType().Should().Be(typeof(Noark4FileReader));
        }

        [Fact]
        public void ShouldThrowExceptionForNoark5()
        {
            Assert.Throws<ArgumentException>(
                () => new FlatFileReaderFactory().GetReader(CreateArchive(ArchiveType.Noark5))
                );
        }
    }
}