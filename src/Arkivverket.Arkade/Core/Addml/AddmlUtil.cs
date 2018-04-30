using System;
using System.IO;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.ExternalModels.Addml;
using Arkivverket.Arkade.Util;
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
            }
            catch (FileNotFoundException e)
            {
                throw new ArkadeException(string.Format(Resources.ExceptionMessages.FileNotFound, fileName), e);
            }
            catch (Exception e)
            {
                throw new ArkadeException(string.Format(Resources.Messages.ExceptionReadingAddmlFile, e.Message), e);
            }
        }

        private static void Validate(string fileContent, string addmlXsd)
        {
            try
            {
                new XmlValidator().Validate(fileContent, addmlXsd);
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
