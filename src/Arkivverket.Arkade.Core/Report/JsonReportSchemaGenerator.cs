using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema.Generation;

namespace Arkivverket.Arkade.Core.Report
{
    public class JsonReportSchemaGenerator : IReportSchemaGenerator
    {
        public void Generate(Type reportType, TextWriter schemaWriter)
        {
            var jsonSchemaGenerator = new JSchemaGenerator();
            jsonSchemaGenerator.GenerationProviders.Add(new StringEnumGenerationProvider());
            jsonSchemaGenerator.Generate(reportType, false).WriteTo(new JsonTextWriter(schemaWriter));
        }
    }
}
