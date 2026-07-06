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
        foreach ((string documentedXmlFileName, IEnumerable<string> documentedSchemaNames) in archiveDetails.DocumentedXmlUnits)
        {
            if (archiveContent.GetFile(documentedXmlFileName) is { } xmlFileInArchive)
            {
                var archiveXmlFile = new ArchiveXmlFile(xmlFileInArchive);
                var archiveXmlSchemas = new List<ArchiveXmlSchema>();
                List<string> documentedNames = documentedSchemaNames.ToList();
                List<string> standardSchemaNames = archiveDetails.StandardXmlUnits[archiveXmlFile.Name].ToList();

                // The ADDML-documented schemas include any custom schemas beside the standard ones
                foreach (string schemaName in documentedNames.Union(standardSchemaNames))
                {
                    // Only ADDML-documented schema files are used from the archive; an undocumented
                    // standard-named schema file is disregarded in favour of the built-in
                    bool isDocumented = documentedNames.Contains(schemaName);

                    if (isDocumented && archiveContent.GetFile(schemaName) is { } schemaFileInArchive)
                    {
                        archiveXmlSchemas.Add(new UserProvidedXmlSchema(schemaFileInArchive));
                    }
                    else
                    {
                        if (isDocumented)
                            Log.Warning(Format(Noark5Messages.FileNotFound, schemaName));

                        if (!standardSchemaNames.Contains(schemaName))
                            continue; // custom schema missing from the archive — no built-in to fall back on

                        string archiveTypeVersion = GetArchiveTypeVersion(archiveDetails);

                        string pathVersionString = "v" + archiveTypeVersion.Replace('.', '_');
                        var xsdResourceSubPath =
                            $"{Format(ArkadeConstants.LocalDirectoryPathNoark5XsdResources, pathVersionString)}";

                        var schemaVersion = new ArkadeBuiltInXmlSchema.Version(archiveTypeVersion, xsdResourceSubPath);
                        archiveXmlSchemas.Add(new ArkadeBuiltInXmlSchema(schemaName, schemaVersion));
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
