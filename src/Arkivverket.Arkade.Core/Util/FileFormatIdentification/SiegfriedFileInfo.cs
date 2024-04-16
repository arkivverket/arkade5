using CsvHelper;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using CsvHelper.Configuration;
using Serilog;

namespace Arkivverket.Arkade.Core.Util.FileFormatIdentification
{
    public class SiegfriedFileInfo : IFileFormatInfo, IEquatable<SiegfriedFileInfo>
    {
        public string FileName { get; }
        public string ByteSize { get; }
        public string FileExtension { get; }
        public string Errors { get; }
        public string Id { get; }
        public string Format { get; }
        public string Version { get; }
        public string MimeType { get; }

        public SiegfriedFileInfo(string fileName, string byteSize, string errors, string id, string format, string version, string mimeType)
        {
            FileName = fileName;
            ByteSize = byteSize;
            FileExtension = Path.GetExtension(fileName);
            Errors = errors;
            Id = id;
            Format = format;
            Version = version;
            MimeType = mimeType;
        }

        public bool Equals(SiegfriedFileInfo other)
        {
            return Id == other?.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is SiegfriedFileInfo info && Equals(info);
        }

        public override int GetHashCode()
        {
            var sb = new StringBuilder();
            sb.Append(FileName);
            sb.Append(ByteSize);
            sb.Append(FileExtension);
            sb.Append(Errors);
            sb.Append(Id);
            sb.Append(Version);
            sb.Append(MimeType);
            return sb.ToString().GetHashCode();
        }

        public static IFileFormatInfo CreateFromString(string siegfriedFormatResult)
        {
            if (siegfriedFormatResult == null)
                return null;

            var parseProblem = string.Empty;

            using var stringReader = new StringReader(siegfriedFormatResult);
            using var csvParser = new CsvParser(stringReader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                BadDataFound = context =>
                {
                    Log.Error($"Bad record: {context.RawRecord}");
                    parseProblem = "Analysis result parse problem";
                }
            });

            csvParser.Read();

            return new SiegfriedFileInfo
            (
                fileName: csvParser.Record[0],
                byteSize: csvParser.Record[1],
                errors: string.Join(", ", csvParser.Record[3], parseProblem).Trim(", "),
                id: csvParser.Record[5],
                format: csvParser.Record[6],
                version: csvParser.Record[7],
                mimeType: csvParser.Record[8]
            );
        }
    }
}
