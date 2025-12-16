using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;
using Serilog;
using static System.String;

namespace Arkivverket.Arkade.Core.Base.Archives;

public class Noark5XmlUnits
{
    private readonly List<ArchiveXmlUnit> _units = [];

    public List<ArchiveXmlUnit> Get() => _units;
    
    public ArchiveXmlUnit Get(string fileName) => _units.FirstOrDefault(xmlUnit => xmlUnit.File.Name.Equals(fileName));

    public Noark5XmlUnits(DirectoryArchiveContent archiveContent, ArchiveDetails archiveDetails)
    {
        foreach (string documentedXmlFileName in archiveDetails.DocumentedXmlUnits.Keys)
        {
            if (archiveContent.GetFile(documentedXmlFileName) is { } xmlFileInArchive)
            {
                var archiveXmlFile = new ArchiveXmlFile(xmlFileInArchive);
                var archiveXmlSchemas = new List<ArchiveXmlSchema>();

                foreach (string standardSchemaName in archiveDetails.StandardXmlUnits[archiveXmlFile.Name])
                {
                    if (archiveContent.GetFile(standardSchemaName) is { } schemaFileInArchive)
                    {
                        archiveXmlSchemas.Add(new UserProvidedXmlSchema(schemaFileInArchive));
                    }
                    else
                    {
                        Log.Warning(Format(Noark5Messages.FileNotFound, standardSchemaName));

                        string archiveTypeVersion = GetArchiveTypeVersion(archiveDetails);

                        string pathVersionString = "v" + archiveTypeVersion.Replace('.', '_');
                        var xsdResourceSubPath =
                            $"{Format(ArkadeConstants.LocalDirectoryPathNoark5XsdResources, pathVersionString)}";

                        var schemaVersion = new ArkadeBuiltInXmlSchema.Version(archiveTypeVersion, xsdResourceSubPath);
                        archiveXmlSchemas.Add(new ArkadeBuiltInXmlSchema(standardSchemaName, schemaVersion));
                    }
                }

                _units.Add(new ArchiveXmlUnit(archiveXmlFile, archiveXmlSchemas));
            }
            else
            {
                Log.Error(Format(Noark5Messages.FileNotFound, documentedXmlFileName));
            }
        }
    }

    private static string GetArchiveTypeVersion(ArchiveDetails archiveDetails)
    {
        if (ArkadeConstants.SupportedNoark5Versions.Contains(archiveDetails.ArchiveStandard))
            return archiveDetails.ArchiveStandard;

        Log.Warning(Format(Noark5Messages.Noark5VersionNotSupportedForBuiltInSchemas, archiveDetails.ArchiveStandard));
        return ArkadeConstants.LatestNoark5Version;
    }
}
