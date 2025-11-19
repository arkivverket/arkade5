using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;

namespace Arkivverket.Arkade.Core.Base.Archives;

public sealed class Noark3Archive : AddmlDefinitionTestedArchive
{
    public override required AddmlXmlUnit AddmlXmlUnit { get; init; }
    public override required AddmlInfo AddmlInfo { get; init; }
    
    [SetsRequiredMembers]
    public Noark3Archive(DirectoryInfo archiveExtractionDirectory, DirectoryInfo processingDirectory) : base(processingDirectory)
    {
        AddmlXmlUnit = new AddmlXmlUnit(null, null); // TODO: Implement!
        AddmlInfo = new AddmlInfo(null, null); // TODO: Implement!
    }

    [SetsRequiredMembers]
    public Noark3Archive(InputDiasPackage inputDiasPackage, DirectoryInfo processingDirectory) : base(processingDirectory)
    {
        AddmlXmlUnit = new AddmlXmlUnit(null, null); // TODO: Implement!
        AddmlInfo = new AddmlInfo(null, null); // TODO: Implement!
    }
}
