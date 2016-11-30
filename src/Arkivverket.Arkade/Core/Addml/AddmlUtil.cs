using System;
using System.IO;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.ExternalModels.Addml;
using Arkivverket.Arkade.Util;
using Arkivverket.Arkade.Test.Core;
using System.Xml.Schema;

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
            string addmlXsd = ResourceUtil.ReadResource(ArkadeConstants.AddmlXsdResource);

            try
            {
                string fileContent = File.ReadAllText(fileName);
                addml addml = ReadFromString(fileContent);

                Validate(fileContent, addmlXsd);

                return new AddmlInfo(addml, new FileInfo(fileName));
            } catch (ArkadeException e)
            {
                string error = fileName + ": " + e.Message;
                throw new ArkadeException(error, e);
            }
        }

        private static void Validate(string fileContent, string addmlXsd)
        {
            try
            {
                XmlUtil.Validate(fileContent, addmlXsd);
            } catch (XmlSchemaException e)
            {
                throw new ArkadeException(e.Message);
            }
        }

        public static AddmlInfo ReadFromBaseDirectory(string fileName)
        {
            return ReadFromFile(AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + fileName);
        }

        private static void Validate(addml addml)
        {

        }

    }
}