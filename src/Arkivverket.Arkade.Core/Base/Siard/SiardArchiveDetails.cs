using System.Collections.Generic;
using System.IO;
using Arkivverket.Arkade.Core.ExternalModels.Metadata;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Siard
{
    public class SiardArchiveDetails : IArchiveDetails
    {
        public string ArchiveCreators => GetArchiveCreators();
        public string ArchivalPeriod => GetArchivalPeriod();
        public string SystemName => GetSystemName();
        public string SystemType => GetSystemType();

        public virtual string ArchiveStandard => _archiveStandard ?? GetArchiveStandardVersion();
        
        public Dictionary<string, IEnumerable<string>> DocumentedXmlUnits => GetDocumentedXmlUnits();
        
        public Dictionary<string, IEnumerable<string>> StandardXmlUnits => GetStandardXmlUnits();
        
        private string _archiveStandard;
        private readonly siardArchive _siardArchive;

        public SiardArchiveDetails(siardArchive siardArchive)
        {
            _siardArchive = siardArchive;
        }

        public IArchiveDetails Create(object siardArchive)
        {
            return new SiardArchiveDetails(siardArchive as siardArchive);
        }

        private string GetArchiveCreators()
        {
            return _siardArchive.archiver;
        }

        private string GetArchivalPeriod()
        {
            return _siardArchive.dataOriginTimespan;
        }

        private string GetSystemName()
        {
            return _siardArchive.dbname;
        }

        private string GetSystemType()
        {
            return $"SIARD {Resources.Report.VersionText} {ArchiveStandard}";
        }

        private string GetArchiveStandardVersion()
        {
            _archiveStandard = _siardArchive.version.ToString();
            return _archiveStandard;
        }

        private Dictionary<string, IEnumerable<string>> GetDocumentedXmlUnits()
        {
            var dict = new Dictionary<string, IEnumerable<string>>();

            foreach (var schema in _siardArchive.schemas)
            {
                foreach (var table in schema.tables)
                {
                    string xmlTableFilePath = Path.Join(schema.folder, table.folder, table.folder + ".xml");
                    dict.Add(xmlTableFilePath, new List<string> {xmlTableFilePath.Replace("xml", "xsd")});
                }
            }

            return dict;
        }

        private static Dictionary<string, IEnumerable<string>> GetStandardXmlUnits()
        {
            return new()
            {
                {
                    ArkadeConstants.SiardMetadataXmlFileName,
                    new[]
                    {
                        ArkadeConstants.SiardMetadataXsdFileName,
                    }
                },
            };
        }
    }
}
