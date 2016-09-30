using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5
{
    class NumberOfClassesInMainClassificationSystemWithoutSubClassesorFolders : BaseTest
    {

        public const string AnalysisClasses = "Simple class structures";

        public NumberOfClassesInMainClassificationSystemWithoutSubClassesorFolders(IArchiveContentReader archiveReader) : base(TestType.Content, archiveReader)
        {
        }


        protected override void Test(Archive archive)
        {
            using (var reader = XmlReader.Create(ArchiveReader.GetContentAsStream(archive)))
            {
                int cntClasses = 0;

                if (reader.ForwardToFirstInstanceOfElement("arkivdel"))
                {
                    if (reader.ForwardToFirstInstanceOfElement("klassifikasjonssystem"))
                    {
                        while (!reader.IsReaderAtElementEndTag("klassifikasjonssystem") && reader.Read())
                        {
                            if (reader.IsNodeTypeAndName(XmlNodeType.Element, "klasse"))
                            {
                                var isCandidateForClassWithoutSubClassAndFolder = true;
                                while (!reader.IsReaderAtElementEndTag("klasse") && reader.Read())
                                {
                                    // This is where we are looking for no sub categories of klasse and mappe
                                    if (reader.IsNodeTypeAndName(XmlNodeType.Element, "klasse"))
                                    {
                                        isCandidateForClassWithoutSubClassAndFolder = false;
                                        while (!reader.IsReaderAtElementEndTag("klasse") && reader.Read())
                                        {
                                        }
                                    }
                                    if (reader.IsNodeTypeAndName(XmlNodeType.Element, "mappe"))
                                    {
                                        isCandidateForClassWithoutSubClassAndFolder = false; while (!reader.IsReaderAtElementEndTag("mappe") && reader.Read())
                                        {

                                        }
                                    }
                                }
                                if (isCandidateForClassWithoutSubClassAndFolder)
                                {
                                    cntClasses++;
                                }
                            }
                        }
                    }
                }

                AddAnalysisResult(AnalysisClasses, cntClasses.ToString());
                TestSuccess($"Antall klasser uten underklasser eller mapper: {cntClasses}.");
            }
        }

    }
}
