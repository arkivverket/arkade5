using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Util.FileFormatIdentification;
using ICSharpCode.SharpZipLib.Tar;

namespace Arkivverket.Arkade.Core.Util
{
    public class FileCounter : IFileCounter
    {
        private readonly List<string> _supportedArchiveFormatExtension = new()
        {
            ".zip", ".tar", ".gz", ".arc", ".warc"
        };

        private readonly IStatusEventHandler _statusEventHandler;
        private readonly BackgroundWorker _fileCounter;

        public FileCounter(IStatusEventHandler statusEventHandler)
        {
            _statusEventHandler = statusEventHandler;
            _fileCounter = new BackgroundWorker();
            _fileCounter.DoWork += FileCounterOnDoWork;
        }

        public void CountFiles(FileFormatScanMode scanMode, string target)
        {
            _fileCounter.RunWorkerAsync(new { ScanMode = scanMode, Target = target });
        }

        private void FileCounterOnDoWork(object sender, DoWorkEventArgs e)
        {
            var scanArgs = e.Argument as dynamic;
            long numberOfFilesToAnalyse = CountNumberOfFilesToAnalyse(scanArgs.ScanMode, scanArgs.Target);
            _statusEventHandler.RaiseEventFormatAnalysisTotalFileCounterFinished(numberOfFilesToAnalyse);
        }

        private long CountNumberOfFilesToAnalyse(FileFormatScanMode scanMode, string target)
        {
            return scanMode switch
            {
                FileFormatScanMode.Directory => CountNumberOfFilesInDirectory(new DirectoryInfo(target)),
                FileFormatScanMode.Archive => CountNumberOfFileEntriesInArchiveFile(target),
                _ => 1
            };
        }

        private long CountNumberOfFilesInDirectory(DirectoryInfo directory)
        {
            IEnumerable<FileInfo> allFiles = directory.EnumerateFiles("*", new EnumerationOptions
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = true,
            });

            long numberOfFilesInDirectory = 0;

            foreach (FileInfo fileInfo in allFiles)
            {
                numberOfFilesInDirectory++;
                if (_supportedArchiveFormatExtension.Contains(fileInfo.Extension) || fileInfo.Attributes.HasFlag(FileAttributes.Archive))
                {
                    numberOfFilesInDirectory += CountNumberOfFileEntriesInArchiveFile(fileInfo.FullName);
                }
            }
            
            return numberOfFilesInDirectory;
        }

        private long CountNumberOfFileEntriesInArchiveFile(string pathToArchiveFile)
        {
            foreach (string supportedExtension in _supportedArchiveFormatExtension)
            {
                try
                {
                    using FileStream archiveFileAsStream = File.OpenRead(pathToArchiveFile);
                    return RunFileEntryCounter(supportedExtension, archiveFileAsStream);
                }
                catch (Exception)
                {
                    // try next supported extension
                }
            }

            return 0;
        }

        private long RunFileEntryCounter(string extension, Stream archiveFileAsStream)
        {
            return extension.ToLower() switch
            {
                ".zip" => CountFileEntriesForZip(archiveFileAsStream),
                ".tar" or ".gz" => CountFileEntriesForTar(archiveFileAsStream),
                ".arc" or ".warc" => 0,
                _ => 0
            };
        }

        private long CountFileEntriesForZip(Stream archiveFileAsStream)
        {
            using var zipArchive = new ZipArchive(archiveFileAsStream);
            long numberOfFilesInArchive = zipArchive.Entries.LongCount(e => e.Name != string.Empty);
            foreach (ZipArchiveEntry entry in zipArchive.Entries.Where(e => e.Name != string.Empty))
            {
                string entryExtension = Path.GetExtension(entry.Name);

                if (_supportedArchiveFormatExtension.Contains(entryExtension))
                {
                    numberOfFilesInArchive += CountFileEntriesForArchiveFileInsideZip(entry, entryExtension);
                }
            }

            return numberOfFilesInArchive;
        }

        private long CountFileEntriesForTar(Stream archiveFileAsStream)
        {
            using var tarInputStream = new TarInputStream(archiveFileAsStream, Encoding.UTF8);
            long counter = 0;
            while (tarInputStream.GetNextEntry() is { } entry)
            {
                if (entry.IsDirectory) continue;

                counter++;

                string entryExtension = Path.GetExtension(entry.Name);

                if (_supportedArchiveFormatExtension.Contains(entryExtension))
                {
                    counter += CountFileEntriesForArchiveFileInsideTar(tarInputStream, entryExtension);
                }
            }

            return counter;
        }

        private long CountFileEntriesForArchiveFileInsideZip(ZipArchiveEntry entry, string entryExtension)
        {
            using Stream entryAsStream = entry.Open();
            return RunFileEntryCounter(entryExtension, entryAsStream);
        }

        private long CountFileEntriesForArchiveFileInsideTar(TarInputStream tarInputStream, string entryExtension)
        {
            using var entryAsStream = new MemoryStream();
            tarInputStream.CopyEntryContents(entryAsStream);
            entryAsStream.Position=0;
            return RunFileEntryCounter(entryExtension, entryAsStream);
        }
    }
}
