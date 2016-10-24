using System;
using System.IO;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.ExternalModels.Addml;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core.Addml
{
    public class AddmlUtil
    {
        public static addml ReadFromString(string xml)
        {
            return SerializeUtil.DeserializeFromString<addml>(xml);
        }

        public static AddmlInfo ReadFromFile(string fileName)
        {
            string fileContent = File.ReadAllText(fileName);
            addml addml = ReadFromString(fileContent);
            return new AddmlInfo(addml, new FileInfo(fileName));
        }

        public static AddmlInfo ReadFromBaseDirectory(string fileName)
        {
            return ReadFromFile(AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + fileName);
        }
    }
}