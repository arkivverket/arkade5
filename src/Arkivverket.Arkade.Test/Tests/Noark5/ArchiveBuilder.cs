using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Arkivverket.Arkade.ExternalModels.Noark5;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class ArchiveBuilder
    {
        private readonly arkiv _archive = new arkiv();
        private readonly List<object> _archiveItems = new List<object>();

        public ArchivePartBuilder WithArchivePart(string title = "This is the default archive part title.")
        {
            var archivePartBuilder = new ArchivePartBuilder(this);

            _archiveItems.Add(archivePartBuilder.ArchivePart(title));
           
            return archivePartBuilder;
        }

        private void AppendItems()
        {
            _archive.Items = _archiveItems.ToArray();
        }

        public Stream Build()
        {
            AppendItems();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            namespaces.Add(string.Empty, "http://www.arkivverket.no/standarder/noark5/arkivstruktur");

            return SerializeUtil.SerializeToStream(_archive, namespaces);
        }


        public class ArchivePartBuilder
        {
            private readonly ArchiveBuilder _archiveBuilder;
            private arkivdel _arkivdel = new arkivdel();

            public ArchivePartBuilder(ArchiveBuilder archiveBuilder)
            {
                _archiveBuilder = archiveBuilder;
            }

            public ArchiveBuilder WithClassificationSystem(string title = "default classification system title")
            {
                _arkivdel.Items = new object[]
                {
                    new klassifikasjonssystem {tittel = title}
                };
                return _archiveBuilder;
            }

            public arkivdel ArchivePart(string title = "default archive part title")
            {
                _arkivdel = new arkivdel {tittel = title};
                return _arkivdel;
            }

            public Stream Build()
            {
                return _archiveBuilder.Build();
            }
        }

        
    }
}