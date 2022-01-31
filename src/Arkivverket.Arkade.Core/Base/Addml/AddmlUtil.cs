using System;
using System.IO;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.ExternalModels.Addml;
using Arkivverket.Arkade.Core.Util;
using System.Xml.Schema;

namespace Arkivverket.Arkade.Core.Base.Addml
{
    public class AddmlUtil
    {
        public static addml ReadFromString(string xml)
        {
            return SerializeUtil.DeserializeFromString<addml>(xml);
        }

        public static AddmlInfo ReadFromFile(string xmlFileName, Stream xmlSchemaStream)
        {
            try
            {
                var addml = SerializeUtil.DeserializeFromFile<addml>(xmlFileName);

                using FileStream xmlStream = File.OpenRead(xmlFileName);

                Validate(xmlStream, xmlSchemaStream);

                return new AddmlInfo(addml, new FileInfo(xmlFileName));
            }
            catch (Exception e)
            {
                throw new ArkadeException(string.Format(Resources.Messages.ExceptionReadingAddmlFile, e.Message), e);
            }
        }

        private static void Validate(Stream xmlStream, Stream xmlSchemaStream)
        {
            try
            {
                new XmlValidator().Validate(xmlStream, xmlSchemaStream);
            } 
            catch (XmlSchemaException e)
            {
                throw new ArkadeException(e.Message);
            }
        }
    }
}
