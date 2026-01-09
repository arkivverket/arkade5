using System.IO;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base;

public class AddmlXmlUnit(ArchiveXmlFile archiveXmlFile, ArchiveXmlSchema archiveXmlSchema)
    : ArchiveXmlUnit(archiveXmlFile, [archiveXmlSchema])
{
    public ArchiveXmlSchema Schema => Schemas[0];

    public void WriteFiles(ArkadeDirectory targetDirectory)
    {
        WriteFile(File.AsStream(), File.Name, targetDirectory);
        WriteFile(Schema.AsStream(), Schema.FileName, targetDirectory);
    }

    private static void WriteFile(Stream sourceFileStream, string sourceFileName, ArkadeDirectory targetDirectory)
    {
        string targetFilePath = targetDirectory.WithFile(sourceFileName).FullName;

        using Stream targetFileStream = System.IO.File.Create(targetFilePath);
        using (sourceFileStream) sourceFileStream.CopyTo(targetFileStream);
    }
}
