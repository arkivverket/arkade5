using System.IO;
using Arkivverket.Arkade.ExternalModels.Addml;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core.Addml.Definitions
{
    public class AddmlInfo
    {
        public addml Addml { get; }
        public FileInfo AddmlFile { get; }
        
        public AddmlInfo(addml addml, FileInfo addmlFile)
        {
            Assert.AssertNotNull(Resources.AddmlMessages.Addml, addml);
            Assert.AssertNotNull(Resources.AddmlMessages.AddmlFile, addmlFile);

            Addml = addml;
            AddmlFile = addmlFile;
        }
    }
}