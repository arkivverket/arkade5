using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Archives;

public sealed class Noark3Archive : AddmlDefinitionTestedArchive
{
    
    [SetsRequiredMembers]
    public Noark3Archive(DirectoryInfo archiveExtractionDirectory, DirectoryInfo processingDirectory) : base(processingDirectory)
    {
        Content = new ArkadeDirectory(archiveExtractionDirectory);
        
        AddmlXmlUnit = new AddmlXmlUnit(null, null); // TODO: Implement!
        AddmlInfo = new AddmlInfo(null, null); // TODO: Implement!
    }

    [SetsRequiredMembers]
    public Noark3Archive(InputDiasPackage inputDiasPackage, DirectoryInfo processingDirectory) : base(processingDirectory)
    {
        Content = inputDiasPackage.WorkingDirectory.ContentWorkDirectory();
        
        AddmlXmlUnit = new AddmlXmlUnit(null, null); // TODO: Implement!
        AddmlInfo = new AddmlInfo(null, null); // TODO: Implement!
    }
}
