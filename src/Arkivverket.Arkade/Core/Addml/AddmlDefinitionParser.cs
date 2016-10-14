using Arkivverket.Arkade.ExternalModels.Addml;
using Arkivverket.Arkade.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Core.Addml
{
    public class AddmlDefinitionParser
    {
        private readonly addml _addml;

        private readonly Dictionary<string, flatFileType> _flatFileTypes = new Dictionary<string, flatFileType>();
        private readonly Dictionary<string, fieldType> _fieldTypes = new Dictionary<string, fieldType>();

        public AddmlDefinitionParser(addml addml)
        {
            Assert.AssertNotNull("addml", addml);
            _addml = addml;

            PopulateFlatFileTypes();
            PopulateFieldTypes();
        }

        private void PopulateFieldTypes()
        {
            fieldType[] fieldTypes = GetFieldTypes();
            if (fieldTypes != null)
            {
                foreach (fieldType fieldType in fieldTypes)
                {
                    _fieldTypes.Add(fieldType.name, fieldType);
                }
            }
        }

        private fieldType[] GetFieldTypes()
        {
            return GetDataset()?.flatFiles?.structureTypes?.fieldTypes;
        }

        private void PopulateFlatFileTypes()
        {
            flatFileType[] flatFileTypes = GetFlatFileTypes();
            if (flatFileTypes != null) {
                foreach (flatFileType flatFileType in flatFileTypes)
                {
                    _flatFileTypes.Add(flatFileType.name, flatFileType);
                }
            }
        }

        private flatFileType[] GetFlatFileTypes()
        {
            return GetDataset()?.flatFiles?.structureTypes?.flatFileTypes;
        }

        public List<AddmlFlatFileDefinition> GetAddmlFlatFileDefinitions()
        {
            List<AddmlFlatFileDefinition> AddmlFlatFileDefinitions = new List<AddmlFlatFileDefinition>();

            flatFileDefinition[] flatFileDefinitions = GetFlatFileDefinitions();
            foreach (flatFileDefinition flatFileDefinition in flatFileDefinitions)
            {
                string fileName = GetFileName(flatFileDefinition.name);
                int recordLength = GetRecordLength(flatFileDefinition);
                string recordSeparator = GetRecordSeparator(flatFileDefinition.typeReference);






                //GetStructureTypes()


                
                //bool isPrimaryKey = IsPrimaryKey(recordDefinition.keys, fieldDefinitionName);


                string name = null;
                var addmlFlatFileDefinition = new AddmlFlatFileDefinition(name, fileName, recordLength);
                AddmlFlatFileDefinitions.Add(addmlFlatFileDefinition);
            }


            //dataset dataset = AddmlFlatFileDefinitions;



            /**
            string fileName = GetFileName(definitionReference);


            flatFile[] flatFiles = GetFlatFiles(dataset);
            foreach (flatFile file in flatFiles)
            {
                file.properties
            }
    */







            return AddmlFlatFileDefinitions;
        }

        private string GetRecordSeparator(string flatFileTypeName)
        {
            flatFileType flatFileType = _flatFileTypes[flatFileTypeName];

            if (flatFileType == null)
            {
                throw new AddmlDefinitionException("No flatFileType with name " + flatFileTypeName);
            }

            // TODO: Add support for delimFileFormat?
            return ((fixedFileFormat)flatFileType.Item).recordSeparator;
        }

        private int GetRecordLength(flatFileDefinition flatFileDefinition)
        {
            recordDefinition recordDefinition = GetRecordDefinition(flatFileDefinition);
            return int.Parse(recordDefinition.fixedLength);
        }

        private bool IsPrimaryKey(keys keys, object fieldDefinitionName)
        {
            foreach (var key in keys.key)
            {
                object o = key.Item;
                if (o is primaryKey)
                {
                    return true;
                }
            }
            return false;
        }

        private recordDefinition GetRecordDefinition(flatFileDefinition flatFileDefinition)
        {
            recordDefinition[] recordDefinitions = flatFileDefinition.recordDefinitions;
            Assert.AssertNotNull("recordDefinitions", recordDefinitions);

            if (recordDefinitions.Length != 1)
            {
                throw new AddmlDefinitionException("recordDefinitions must contain exactly one element. Found " + recordDefinitions.Length + " elements");
            }

            return recordDefinitions[0];
        }


        private string GetFileName(string definitionReference)
        {
            flatFile[] flatFiles = GetFlatFiles();
            foreach (flatFile file in flatFiles)
            {
                if (file.properties != null)
                {
                    foreach (property property in file.properties)
                    {
                        if (definitionReference.Equals(property.name))
                        {
                            return property.value;
                        }
                    }
                }
            }
            return null;
        }

        private flatFile[] GetFlatFiles()
        {
            var flatFiles = GetDataset().flatFiles;
            Assert.AssertNotNull("flatFiles", flatFiles);
            return flatFiles.flatFile;
        }

        private flatFileDefinition[] GetFlatFileDefinitions()
        {
            var flatFiles = GetDataset().flatFiles;
            Assert.AssertNotNull("flatFiles", flatFiles);

            var flatFileDefinitions = flatFiles.flatFileDefinitions;
            Assert.AssertNotNull("flatFileDefinitions", flatFileDefinitions);

            return flatFileDefinitions;
        }

        private dataset GetDataset()
        {
            dataset[] datasets = _addml.dataset;
            Assert.AssertNotNull("dataset", datasets);

            if (datasets.Length != 1)
            {
                throw new AddmlDefinitionException("dataset must contain exactly one element. Found " + datasets.Length + " elements");
            }

            return datasets[0];
        }

    }
}
