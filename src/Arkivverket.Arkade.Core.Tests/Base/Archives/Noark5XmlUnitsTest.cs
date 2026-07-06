using System;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Archives;
using Arkivverket.Arkade.Core.ExternalModels.Addml;
using Arkivverket.Arkade.Core.Util;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base.Archives;

public class Noark5XmlUnitsTest
{
    [Fact]
    public void Uses_user_provided_schemas_when_documented_and_present()
    {
        ArchiveDetails archiveDetails = CreateArchiveDetails(
            ArkadeConstants.LatestNoark5Version,
            ArkadeConstants.ArkivstrukturXmlFileName,
            ArkadeConstants.ArkivstrukturXsdFileName, ArkadeConstants.MetadatakatalogXsdFileName
        );

        DirectoryArchiveContent content = CreateContentDirectory(dir =>
        {
            // XML file present
            File.WriteAllText(Path.Combine(dir.FullName, ArkadeConstants.ArkivstrukturXmlFileName), "<root/>");

            // Both documented schemas present
            File.WriteAllText(Path.Combine(dir.FullName, ArkadeConstants.ArkivstrukturXsdFileName), "");
            File.WriteAllText(Path.Combine(dir.FullName, ArkadeConstants.MetadatakatalogXsdFileName), "");
        });

        var units = new Noark5XmlUnits(content, archiveDetails);

        ArchiveXmlUnit unit = units.Get(ArkadeConstants.ArkivstrukturXmlFileName);
        unit.Should().NotBeNull();
        unit!.File.Name.Should().Be(ArkadeConstants.ArkivstrukturXmlFileName);

        // Expect schemas to be user-provided (i.e., from files on disk)
        unit.Schemas.Should().HaveCount(2);
        unit.Schemas.Select(s => s.GetType().Name).Should()
            .OnlyContain(t => t == nameof(UserProvidedXmlSchema));
        unit.Schemas.Select(s => s.Name).Should().BeEquivalentTo(
            new[] { ArkadeConstants.ArkivstrukturXsdFileName, ArkadeConstants.MetadatakatalogXsdFileName });
    }

    [Fact]
    public void Disregards_undocumented_schema_files_present_in_content()
    {
        ArchiveDetails archiveDetails = CreateArchiveDetails(
            ArkadeConstants.LatestNoark5Version,
            ArkadeConstants.ArkivstrukturXmlFileName
        );

        DirectoryArchiveContent content = CreateContentDirectory(dir =>
        {
            // XML file present
            File.WriteAllText(Path.Combine(dir.FullName, ArkadeConstants.ArkivstrukturXmlFileName), "<root/>");

            // Both standard schemas present on disk, but neither is documented in the addml
            File.WriteAllText(Path.Combine(dir.FullName, ArkadeConstants.ArkivstrukturXsdFileName), "");
            File.WriteAllText(Path.Combine(dir.FullName, ArkadeConstants.MetadatakatalogXsdFileName), "");
        });

        var units = new Noark5XmlUnits(content, archiveDetails);

        ArchiveXmlUnit unit = units.Get(ArkadeConstants.ArkivstrukturXmlFileName);
        unit.Should().NotBeNull();

        // Undocumented schema files are not part of the declared contract: the built-ins apply
        unit!.Schemas.Should().HaveCount(2);
        unit.Schemas.Select(s => s.GetType().Name).Should()
            .OnlyContain(t => t == nameof(ArkadeBuiltInXmlSchema));
        unit.Schemas.Select(s => s.Name).Should().BeEquivalentTo(
            new[] { ArkadeConstants.ArkivstrukturXsdFileName, ArkadeConstants.MetadatakatalogXsdFileName });
    }

    [Fact]
    public void Falls_back_to_builtin_schemas_when_missing_in_content()
    {
        ArchiveDetails archiveDetails = CreateArchiveDetails(
            ArkadeConstants.LatestNoark5Version,
            ArkadeConstants.ArkivstrukturXmlFileName
        );

        DirectoryArchiveContent content = CreateContentDirectory(dir =>
        {
            // Only XML file, schemas are missing
            File.WriteAllText(Path.Combine(dir.FullName, ArkadeConstants.ArkivstrukturXmlFileName), "<root/>");
        });

        var units = new Noark5XmlUnits(content, archiveDetails);

        ArchiveXmlUnit unit = units.Get(ArkadeConstants.ArkivstrukturXmlFileName);
        unit.Should().NotBeNull();
        unit!.Schemas.Should().HaveCount(2);
        unit.Schemas.Select(s => s.GetType().Name).Should()
            .OnlyContain(t => t == nameof(ArkadeBuiltInXmlSchema));
        unit.Schemas.Select(s => s.Name).Should().BeEquivalentTo(
            new[] { ArkadeConstants.ArkivstrukturXsdFileName, ArkadeConstants.MetadatakatalogXsdFileName });
    }

    [Fact]
    public void Skips_documented_unit_when_xml_file_missing()
    {
        ArchiveDetails archiveDetails = CreateArchiveDetails(
            ArkadeConstants.LatestNoark5Version,
            ArkadeConstants.ArkivuttrekkXmlFileName
        );

        DirectoryArchiveContent content = CreateContentDirectory(_ =>
        {
            /* no files created */
        });

        var units = new Noark5XmlUnits(content, archiveDetails);

        units.Get().Should().BeEmpty();
        units.Get(ArkadeConstants.ArkivuttrekkXmlFileName).Should().BeNull();
    }

    private static ArchiveDetails CreateArchiveDetails(string archiveStandardVersion,
        string documentedXmlFile, params string[] documentedSchemaNames)
    {
        string[] documentedXmlFiles = [documentedXmlFile];

        // Root dataObject with archive info and nested dataObjects for each documented xml file
        var root = new dataObject
        {
            properties =
            [
                new property
                {
                    name = "info",
                    properties =
                    [
                        new property
                        {
                            name = "type",
                            properties = [new property { name = "version", value = archiveStandardVersion }]
                        }
                    ]
                }
            ],
            dataObjects = new dataObjects
            {
                dataObject = documentedXmlFiles.Select(fileName => new dataObject
                {
                    properties =
                    [
                        new property { name = "file", properties = [new property { name = "name", value = fileName }] },
                        .. documentedSchemaNames.Select(schemaName => new property
                        {
                            name = "schema",
                            properties =
                            [
                                new property
                                {
                                    name = "file",
                                    properties = [new property { name = "name", value = schemaName }]
                                }
                            ]
                        })
                    ]
                }).ToArray()
            }
        };

        var addml = new addml
        {
            dataset =
            [
                new dataset
                {
                    dataObjects = new dataObjects
                    {
                        dataObject = [root]
                    }
                }
            ]
        };

        return new ArchiveDetails(addml);
    }

    private static DirectoryArchiveContent CreateContentDirectory(Action<DirectoryInfo> arrange)
    {
        string tempPath = Path.Combine(Path.GetTempPath(), "Arkade_Noark5XmlUnitsTest_" + Guid.NewGuid());
        DirectoryInfo dir = Directory.CreateDirectory(tempPath);
        arrange(dir);
        return new DirectoryArchiveContent(dir);
    }
}
