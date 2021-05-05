using System.IO;
using Arkivverket.Arkade.Core.ExternalModels.Addml;

namespace Arkivverket.Arkade.Core.Base.Addml.Definitions
{
    public class AddmlInfo
    {
        public addml Addml { get; }
        public FileInfo AddmlFile { get; }
        
        public AddmlInfo(addml addml, FileInfo addmlFile)
        {
            Addml = addml;
            AddmlFile = addmlFile;
        }
    }
}