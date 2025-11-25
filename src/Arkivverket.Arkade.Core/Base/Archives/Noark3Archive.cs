using System.Diagnostics.CodeAnalysis;
using System.IO;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Util;
using Serilog;
using static Arkivverket.Arkade.Core.Base.ArkadeBuiltInXmlSchema;
using static Arkivverket.Arkade.Core.Util.ArkadeConstants;

namespace Arkivverket.Arkade.Core.Base.Archives;

public sealed class Noark3Archive : AddmlDefinitionTestedArchive
{
    [SetsRequiredMembers]
    public Noark3Archive(DirectoryInfo archiveExtractionDirectory, DirectoryInfo processingDirectory) : base(processingDirectory, archiveExtractionDirectory)
    {
        AddmlXmlUnit = SetupAddmlXmlUnit();

        // using Stream xmlSchemaStream = AddmlXmlUnit.HasNoDefinedSchema()
        //     ? ResourceUtil.GetResourceAsStream(ArkadeConstants.AddmlXsdResource)  // TODO: Use AsStream() when re-written to handle non-Noark5 schemas
        //     : AddmlXmlUnit.Schema.AsStream(); 

        AddmlInfo = AddmlUtil.ReadFromFile(AddmlXmlUnit.File.FullName, AddmlXmlUnit.Schema.AsStream());

        Details = new ArchiveDetails(AddmlInfo.Addml);
    }

    [SetsRequiredMembers]
    public Noark3Archive(InputDiasPackage inputDiasPackage, DirectoryInfo processingDirectory) : base(processingDirectory, inputDiasPackage)
    {
        AddmlXmlUnit = new AddmlXmlUnit(null, null); // TODO: Implement!
        AddmlInfo = new AddmlInfo(null, null); // TODO: Implement!
    }

    private new AddmlXmlUnit SetupAddmlXmlUnit()
    {
        FileInfo addmlFileInfo = Content.WithFile(AddmlXmlFileName);

        if (!addmlFileInfo.Exists)
        {
            Log.Error("No addml file found in Noark3 archive."); // TODO: Throw exception instead?
            return null;
        }

        var addmlXmlFile = new ArchiveXmlFile(addmlFileInfo);

        FileInfo addmlXsdFileInfo = Content.WithFile(AddmlXsdFileName);

        ArchiveXmlSchema addmlSchema = addmlXsdFileInfo.Exists
            ? new UserProvidedXmlSchema(addmlXsdFileInfo)
            : new ArkadeBuiltInXmlSchema(AddmlXsdFileName, new Version(BuiltInAddmlSchemaVersion));

        return new AddmlXmlUnit(addmlXmlFile, addmlSchema);
    }
}
