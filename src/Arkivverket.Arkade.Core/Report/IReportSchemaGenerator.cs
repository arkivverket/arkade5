using System;
using System.IO;

namespace Arkivverket.Arkade.Core.Report
{
    public interface IReportSchemaGenerator
    {
        void Generate(Type reportType, TextWriter schemaWriter);
    }
}
