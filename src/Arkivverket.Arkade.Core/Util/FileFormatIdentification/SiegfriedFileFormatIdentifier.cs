using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Arkivverket.Arkade.Core.Resources;
using CsvHelper;
using Serilog;

namespace Arkivverket.Arkade.Core.Util.FileFormatIdentification
{
    public class SiegfriedFileFormatIdentifier : IFileFormatIdentifier
    {
        private static SiegfriedProcessRunner _processRunner;

        public SiegfriedFileFormatIdentifier(SiegfriedProcessRunner siegfriedProcessRunner)
        {
            _processRunner = siegfriedProcessRunner;
        }
        
        public IEnumerable<IFileFormatInfo> IdentifyFormats(string target, FileFormatScanMode scanMode)
        {
            Process siegfriedProcess = _processRunner.SetupSiegfriedProcess(scanMode, target);

            long numberOfFilesToAnalyse = CalculateNumberOfFilesToAnalyse(scanMode, target);

            IEnumerable<string> siegfriedResult = _processRunner.Run(siegfriedProcess, numberOfFilesToAnalyse);

            int siegfriedCloseStatus = ExternalProcessManager.Close(siegfriedProcess.Id);
            return siegfriedCloseStatus switch
            {
                -1 => throw new SiegfriedFileFormatIdentifierException("Process does not exist"),
                1 => throw new SiegfriedFileFormatIdentifierException("Process was terminated"),
                _ => GetSiegfriedFileInfoObjects(siegfriedResult)
            };
        }
        
        public IEnumerable<IFileFormatInfo> IdentifyFormats(IEnumerable<KeyValuePair<string, IEnumerable<byte>>> filePathsAndByteContent)
        {
            List<Task<IFileFormatInfo>> fileFormatTasks = new();

            foreach (var filePathAndByteContent in filePathsAndByteContent)
            {
                fileFormatTasks.Add(IdentifyFormatAsync(filePathAndByteContent));
            }

            return Task.WhenAll(fileFormatTasks).Result;
        }

        private async Task<IFileFormatInfo> IdentifyFormatAsync(KeyValuePair<string, IEnumerable<byte>> filePathAndByteContent)
        {
            return await Task.Run(() => IdentifyFormat(filePathAndByteContent));
        }

        public IFileFormatInfo IdentifyFormat(FileInfo file)
        {
            const FileFormatScanMode scanMode = FileFormatScanMode.File;

            Process siegfriedProcess = _processRunner.SetupSiegfriedProcess(scanMode, file.FullName);

            string siegfriedResult = _processRunner.RunOnFile(siegfriedProcess);

            ExternalProcessManager.Close(siegfriedProcess);

            return GetSiegfriedFileInfoObject(siegfriedResult);
        }

        public IFileFormatInfo IdentifyFormat(KeyValuePair<string, IEnumerable<byte>> filePathAndByteContent)
        {
            if (filePathAndByteContent.Value == null)
                return FileFormatInfoFactory.Create(filePathAndByteContent.Key,
                    Resources.SiardMessages.InlinedLobContentHasUnsupportedEncoding, "N/A", "N/A", "N/A", "N/A");

            const FileFormatScanMode scanMode = FileFormatScanMode.Stream;

            Process siegfriedProcess = _processRunner.SetupSiegfriedProcess(scanMode, string.Empty);


            try
            {
                string siegfriedResult = _processRunner.RunOnByteArray(siegfriedProcess, filePathAndByteContent);

                return GetSiegfriedFileInfoObject(siegfriedResult);
            }
            catch (Exception e)
            {
                Log.Debug(e.ToString());
                Log.Error($"Was not able to analyse {filePathAndByteContent.Key} - please see logfile for details.");
                return FileFormatInfoFactory.Create(filePathAndByteContent.Key,
                    SiardMessages.ErrorMessage, "N/A", "N/A", "N/A", "N/A");
            }
            finally
            {
                ExternalProcessManager.Close(siegfriedProcess);
            }
        }

        private static IEnumerable<SiegfriedFileInfo> GetSiegfriedFileInfoObjects(IEnumerable<string> formatInfoSet)
        {
            return formatInfoSet.Skip(1).Select(GetSiegfriedFileInfoObject);
        }

        private static SiegfriedFileInfo GetSiegfriedFileInfoObject(string siegfriedFormatResult)
        {
            if (siegfriedFormatResult == null)
                return null;

            using (var stringReader = new StringReader(siegfriedFormatResult))
            using (var csvParser = new CsvParser(stringReader, CultureInfo.InvariantCulture))
            {
                csvParser.Read();

                return new SiegfriedFileInfo
                (
                    fileName: csvParser.Record[0],
                    errors: csvParser.Record[3],
                    id: csvParser.Record[5],
                    format: csvParser.Record[6],
                    version: csvParser.Record[7],
                    mimeType: csvParser.Record[8]
                );
            }
        }

        private long CalculateNumberOfFilesToAnalyse(FileFormatScanMode scanMode, string target)
        {
            if (scanMode is FileFormatScanMode.Directory)
                return CalculateNumberOfFilesToAnalyse(new DirectoryInfo(target));

            return 1;
        }

        private long CalculateNumberOfFilesToAnalyse(DirectoryInfo directory)
        {
            return directory.EnumerateFiles("*", new EnumerationOptions
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = true,
            }).LongCount();
        }
    }

    public class SiegfriedFileFormatIdentifierException : Exception
    {
        public SiegfriedFileFormatIdentifierException()
        {
        }

        public SiegfriedFileFormatIdentifierException(string message)
            : base(message)
        {
        }

        public SiegfriedFileFormatIdentifierException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
