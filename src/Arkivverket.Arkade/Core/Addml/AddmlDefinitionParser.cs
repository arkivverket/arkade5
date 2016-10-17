using Arkivverket.Arkade.ExternalModels.Addml;
using Arkivverket.Arkade.Util;
using System.Collections.Generic;
using System;

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
                string name = flatFileDefinition.name;
                string fileName = GetFileName(flatFileDefinition.name);
                int recordLength = GetRecordLength(flatFileDefinition);
                string recordSeparator = GetRecordSeparator(flatFileDefinition.typeReference);
                string charset = GetCharset(flatFileDefinition.typeReference);

                AddmlFlatFileDefinition addmlFlatFileDefinition =
                    new AddmlFlatFileDefinition(name, fileName, recordLength, recordSeparator, charset);

                AddAddmlFieldDefinitions(addmlFlatFileDefinition, flatFileDefinition);


                AddmlFlatFileDefinitions.Add(addmlFlatFileDefinition);
            }

            return AddmlFlatFileDefinitions;
        }

        private void AddAddmlFieldDefinitions(AddmlFlatFileDefinition addmlFlatFileDefinition, flatFileDefinition flatFileDefinition)
        {

            List<fieldDefinition> fieldDefinitions = GetFieldDefinitions(flatFileDefinition);
            recordDefinition recordDefinition = GetRecordDefinition(flatFileDefinition);

            foreach (fieldDefinition fieldDefinition in fieldDefinitions)
            {
                string name = fieldDefinition.name;
                int? startPosition = GetStartPosition(fieldDefinition);
                int? fixedLength = GetFixedLength(fieldDefinition);
                fieldType fieldType = GetFieldType(fieldDefinition.typeReference);
                string fieldTypeString = fieldType.dataType;
                bool isPrimaryKey = IsPrimaryKey(recordDefinition.keys, name);
                bool isUnique = IsUnique(fieldDefinition);
                bool isNullable = IsNullable(fieldDefinition);
                int? minLength = GetMinLength(fieldDefinition);
                int? maxLength = GetMaxLength(fieldDefinition);
                AddmlFieldDefinition foreignKey = GetForeignKey(fieldDefinition);

                addmlFlatFileDefinition.AddAddmlFieldDefinition(
                        name, startPosition, fixedLength, fieldTypeString, isPrimaryKey, isUnique, isNullable, minLength, maxLength,
                        foreignKey);
            }
        }

        private int? GetMaxLength(fieldDefinition fieldDefinition)
        {
            return fieldDefinition.maxLength == null ? (int?)null : int.Parse(fieldDefinition.maxLength);
        }

        private int? GetFixedLength(fieldDefinition fieldDefinition)
        {
            return fieldDefinition.fixedLength == null ? (int?)null : int.Parse(fieldDefinition.fixedLength);
        }

        private int? GetStartPosition(fieldDefinition fieldDefinition)
        {
            return fieldDefinition.startPos == null ? (int?)null : int.Parse(fieldDefinition.startPos);
        }

        private int? GetMinLength(fieldDefinition fieldDefinition)
        {
            return fieldDefinition.minLength == null ? (int?)null : int.Parse(fieldDefinition.minLength);
        }

        private AddmlFieldDefinition GetForeignKey(fieldDefinition fieldDefinition)
        {
            // TODO!!!
            return null;
        }

        private bool IsNullable(fieldDefinition fieldDefinition)
        {
            return fieldDefinition.notNull == null;
        }

        private bool IsUnique(fieldDefinition fieldDefinition)
        {
            return fieldDefinition.unique != null;
        }

        private List<fieldDefinition> GetFieldDefinitions(flatFileDefinition flatFileDefinition)
        {
            return new List<fieldDefinition>(GetRecordDefinition(flatFileDefinition).fieldDefinitions);
        }

        private string GetCharset(string flatFileTypeName)
        {
            return GetFlatFileType(flatFileTypeName).charset;
        }

        private string GetRecordSeparator(string flatFileTypeName)
        {
            // TODO: Add support for delimFileFormat?
            return ((fixedFileFormat) GetFlatFileType(flatFileTypeName).Item).recordSeparator;
        }

        private flatFileType GetFlatFileType(string flatFileTypeName)
        {
            flatFileType flatFileType = _flatFileTypes[flatFileTypeName];

            if (flatFileType == null)
            {
                throw new AddmlDefinitionException("No flatFileType with name " + flatFileTypeName);
            }

            return flatFileType;
        }

        private fieldType GetFieldType(string typeReference)
        {
            // TODO: Create FieldType object
            fieldType fieldType = _fieldTypes[typeReference];

            if (fieldType == null)
            {
                throw new AddmlDefinitionException("No FieldType with name " + typeReference);
            }

            return fieldType;
        }


        private int GetRecordLength(flatFileDefinition flatFileDefinition)
        {
            recordDefinition recordDefinition = GetRecordDefinition(flatFileDefinition);
            return int.Parse(recordDefinition.fixedLength);
        }

        private bool IsPrimaryKey(key[] keys, object fieldDefinitionName)
        {
            if (keys != null) {
                foreach (var key in keys)
                {
                    object o = key.Item;
                    if (o is primaryKey)
                    {
                        return true;
                    }
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
