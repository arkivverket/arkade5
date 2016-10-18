using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arkivverket.Arkade.ExternalModels.Addml;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core.Addml
{
    public class AddmlUtil
    {
        public static addml ReadFromFile(string fileName)
        {
            string fileContent = File.ReadAllText(fileName);
            return SerializeUtil.DeserializeFromString<addml>(fileContent);
        }
    }
}