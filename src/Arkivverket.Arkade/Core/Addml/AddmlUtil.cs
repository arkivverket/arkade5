using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.ExternalModels.Addml;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core.Addml
{
    public class AddmlUtil
    {
        public static AddmlInfo ReadFromFile(string fileName)
        {
            string fileContent = File.ReadAllText(fileName);
            addml addml = SerializeUtil.DeserializeFromString<addml>(fileContent);

            return new AddmlInfo(addml, new FileInfo(fileName));
        }

        public static AddmlInfo ReadFromBaseDirectory(string fileName)
        {
            return ReadFromFile(AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + fileName);
        }
    }
}