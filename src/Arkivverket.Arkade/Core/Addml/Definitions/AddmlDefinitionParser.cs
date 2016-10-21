using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Addml.Definitions.DataTypes;
using Arkivverket.Arkade.ExternalModels.Addml;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core.Addml.Definitions
{
    // TODO: This class should be split in AddmlFieldDefinitionParser AddmlRecordDefinitionParser, etc
    public class AddmlDefinitionParser
    {
        private readonly AddmlInfo _addmlInfo;

        private readonly Dictionary<string, flatFileType> _flatFileTypes = new Dictionary<string, flatFileType>();
        private readonly Dictionary<string, DataType> _fieldTypes = new Dictionary<string, DataType>();

        private readonly Dictionary<FieldIndex, AddmlFieldDefinition> _allFieldDefinitions =
            new Dictionary<FieldIndex, AddmlFieldDefinition>();

        public AddmlDefinitionParser(AddmlInfo addmlInfo)
        {
            Assert.AssertNotNull("addmlInfo", addmlInfo);
            _addmlInfo = addmlInfo;

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
                    DataType newDataType = CreateFieldType(fieldType);
                    _fieldTypes.Add(fieldType.name, newDataType);
                }
            }
        }

        private DataType CreateFieldType(fieldType fieldType)
        {
            switch (fieldType.dataType)
            {
                case "string":
                    return new StringDataType(fieldType.fieldFormat);
                case "integer":
                    return new IntegerDataType(fieldType.fieldFormat);
                case "float":
                    return new FloatDataType(fieldType.fieldFormat);
                case "date":
                    return new DateDataType(fieldType.fieldFormat);
                case "boolean":
                    return new BooleanDataType(fieldType.fieldFormat);
                case "link":
                    return new LinkDataType();
                default:
                    throw new AddmlDefinitionParseException("Unknown datatype " + fieldType.dataType);
            }
        }

        private fieldType[] GetFieldTypes()
        {
            return GetDataset()?.flatFiles?.structureTypes?.fieldTypes;
        }

        private void PopulateFlatFileTypes()
        {
            flatFileType[] flatFileTypes = GetFlatFileTypes();
            if (flatFileTypes != null)
            {
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

        public AddmlDefinition GetAddmlDefinition()
        {
            List<AddmlFlatFileDefinition> addmlFlatFileDefinitions = GetAddmlFlatFileDefinitions();
            return new AddmlDefinition(addmlFlatFileDefinitions);
        }

        private List<AddmlFlatFileDefinition> GetAddmlFlatFileDefinitions()
        {
            List<AddmlFlatFileDefinition> addmlFlatFileDefinitions = new List<AddmlFlatFileDefinition>();

            flatFileDefinition[] flatFileDefinitions = GetFlatFileDefinitions();
            foreach (flatFileDefinition flatFileDefinition in flatFileDefinitions)
            {
                string name = flatFileDefinition.name;
                string recordSeparator = GetRecordSeparator(flatFileDefinition.typeReference);
                string fileName = GetFileName(flatFileDefinition.name);
                FileInfo fileInfo =
                    new FileInfo(_addmlInfo.AddmlFile.DirectoryName + Path.DirectorySeparatorChar + fileName);
                string charset = GetCharset(flatFileDefinition.typeReference);
                List<string> flatFileProcesses = GetFlatFileProcessNames(flatFileDefinition.name);

                AddmlFlatFileDefinition addmlFlatFileDefinition =
                    new AddmlFlatFileDefinition(name, fileName, fileInfo, recordSeparator, charset,
                        flatFileProcesses);

                AddAddmlFieldDefinitions(addmlFlatFileDefinition, flatFileDefinition);


                addmlFlatFileDefinitions.Add(addmlFlatFileDefinition);
            }

            return addmlFlatFileDefinitions;
        }

        private List<string> GetFlatFileProcessNames(string flatFileDefinitionName)
        {
            List<string> ret = new List<string>();
            flatFileProcesses p = GetFlatFileProcesses(flatFileDefinitionName);
            if (p?.processes != null)
            {
                foreach (process process in p.processes)
                {
                    ret.Add(process.name);
                }
            }
            return ret;
        }

        private flatFileProcesses GetFlatFileProcesses(string flatFileDefinitionName)
        {
            flatFileProcesses[] flatFileProcesses = GetDataset()?.flatFiles?.flatFileProcesses;
            if (flatFileProcesses == null)
            {
                return null;
            }

            foreach (flatFileProcesses p in flatFileProcesses)
            {
                if (flatFileDefinitionName.Equals(p.flatFileReference))
                {
                    return p;
                }
            }

            return null;
        }

        private List<string> GetFieldProcessNames(string flatFileDefinitionName, string recordDefinitionName,
            string fieldDefinitionName)
        {
            flatFileProcesses flatFileProcesses = GetFlatFileProcesses(flatFileDefinitionName);
            recordProcesses recordProcesses = GetRecordProcesses(flatFileProcesses, recordDefinitionName);
            fieldProcesses fieldProcesses = GetFieldProcesses(recordProcesses, fieldDefinitionName);

            return fieldProcesses?.processes?.Select(p => p.name).ToList() ?? new List<string>();
        }

        private List<string> GetRecordProcessNames(string flatFileDefinitionName, string recordDefinitionName)
        {
            flatFileProcesses flatFileProcesses = GetFlatFileProcesses(flatFileDefinitionName);
            recordProcesses recordProcesses = GetRecordProcesses(flatFileProcesses, recordDefinitionName);

            return recordProcesses?.processes?.Select(p => p.name).ToList() ?? new List<string>();
        }

        private fieldProcesses GetFieldProcesses(recordProcesses recordProcesses, string fieldDefinitionName)
        {
            if (recordProcesses?.fieldProcesses != null)
            {
                foreach (fieldProcesses p in recordProcesses.fieldProcesses)
                {
                    if (p.definitionReference.Equals(fieldDefinitionName))
                    {
                        return p;
                    }
                }
            }
            return null;
        }

        private recordProcesses GetRecordProcesses(flatFileProcesses flatFileProcesses, string recordDefinitionName)
        {
            if (flatFileProcesses?.recordProcesses != null)
            {
                foreach (recordProcesses p in flatFileProcesses.recordProcesses)
                {
                    if (p.definitionReference.Equals(recordDefinitionName))
                    {
                        return p;
                    }
                }
            }
            return null;
        }


        private void AddAddmlFieldDefinitions(AddmlFlatFileDefinition addmlFlatFileDefinition,
            flatFileDefinition flatFileDefinition)
        {
            List<recordDefinition> recordDefinitions = GetRecordDefinitions(flatFileDefinition);
            foreach (recordDefinition recordDefinition in recordDefinitions)
            {
                string recordDefinitionName = recordDefinition.name;
                int recordLength = GetRecordLength(recordDefinition);
                List<string> recordProcesses = GetRecordProcessNames(addmlFlatFileDefinition.Name, recordDefinition.name);

                AddmlRecordDefinition addmlRecordDefinition = 
                    addmlFlatFileDefinition.AddAddmlRecordDefinition(recordDefinitionName, recordLength, recordProcesses);

                List<fieldDefinition> fieldDefinitions = GetFieldDefinitions(recordDefinition);
                foreach (fieldDefinition fieldDefinition in fieldDefinitions)
                {
                    string name = fieldDefinition.name;
                    int? startPosition = GetStartPosition(fieldDefinition);
                    int? fixedLength = GetFixedLength(fieldDefinition);
                    DataType dataType = GetFieldType(fieldDefinition.typeReference);
                    bool isPartOfPrimaryKey = IsPartOfPrimaryKey(recordDefinition, fieldDefinition);
                    bool isUnique = IsUnique(fieldDefinition);
                    bool isNullable = IsNullable(fieldDefinition);
                    int? minLength = GetMinLength(fieldDefinition);
                    int? maxLength = GetMaxLength(fieldDefinition);
                    AddmlFieldDefinition foreignKeyReference = GetForeignKeyReference(recordDefinition, fieldDefinition);
                    List<string> processes = GetFieldProcessNames(flatFileDefinition.name, recordDefinition.name,
                        fieldDefinition.name);

                    AddmlFieldDefinition addAddmlFieldDefinition = addmlRecordDefinition.AddAddmlFieldDefinition(
                        name, startPosition, fixedLength, dataType, isUnique, isNullable, minLength,
                        maxLength, foreignKeyReference, processes, isPartOfPrimaryKey);

                    _allFieldDefinitions.Add(new FieldIndex(flatFileDefinition, recordDefinition, fieldDefinition),
                        addAddmlFieldDefinition);
                }
            }
        }

        private int? GetMaxLength(fieldDefinition fieldDefinition)
        {
            return fieldDefinition.maxLength == null ? (int?) null : int.Parse(fieldDefinition.maxLength);
        }

        private int? GetFixedLength(fieldDefinition fieldDefinition)
        {
            return fieldDefinition.fixedLength == null ? (int?) null : int.Parse(fieldDefinition.fixedLength);
        }

        private int? GetStartPosition(fieldDefinition fieldDefinition)
        {
            return fieldDefinition.startPos == null ? (int?) null : int.Parse(fieldDefinition.startPos);
        }

        private int? GetMinLength(fieldDefinition fieldDefinition)
        {
            return fieldDefinition.minLength == null ? (int?) null : int.Parse(fieldDefinition.minLength);
        }


        private bool IsNullable(fieldDefinition fieldDefinition)
        {
            return fieldDefinition.notNull == null;
        }

        private bool IsUnique(fieldDefinition fieldDefinition)
        {
            return fieldDefinition.unique != null;
        }

        private List<fieldDefinition> GetFieldDefinitions(recordDefinition recordDefinition)
        {
            return new List<fieldDefinition>(recordDefinition.fieldDefinitions);
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
                throw new AddmlDefinitionParseException("No flatFileType with name " + flatFileTypeName);
            }

            return flatFileType;
        }

        private DataType GetFieldType(string typeReference)
        {
            DataType dataType = _fieldTypes[typeReference];

            if (dataType == null)
            {
                throw new AddmlDefinitionParseException("No FieldType with name " + typeReference);
            }

            return dataType;
        }


        private int GetRecordLength(recordDefinition recordDefinition)
        {
            return int.Parse(recordDefinition.fixedLength);
        }

        private AddmlFieldDefinition GetForeignKeyReference(recordDefinition recordDefinition,
            fieldDefinition fieldDefinition)
        {
            key[] keys = recordDefinition.keys;
            if (keys != null)
            {
                foreach (key key in keys)
                {
                    fieldDefinitionReference[] keyFieldDefinitionReferences = key.fieldDefinitionReferences;
                    foreach (fieldDefinitionReference fieldDefinitionReference in keyFieldDefinitionReferences)
                    {
                        if (fieldDefinitionReference.name.Equals(fieldDefinition.name))
                        {
                            object o = key.Item;
                            if (o is foreignKey)
                            {
                                foreignKey f = (foreignKey) o;
                                List<FieldIndex> indexes = GetForeignKeyIndexes(f);

                                // TODO: Is it possible to have reference to more than one AddmlFieldDefinition?
                                if (indexes.Count != 1)
                                {
                                    throw new AddmlDefinitionParseException(
                                        "foreignKey must reference exactly one fieldDefinitionReference. " + f);
                                }

                                FieldIndex index = indexes[0];
                                if (!_allFieldDefinitions.ContainsKey(index))
                                {
                                    return null;
                                }

                                return _allFieldDefinitions[index];
                            }
                        }
                    }
                }
            }

            return null;
        }

        private List<FieldIndex> GetForeignKeyIndexes(foreignKey foreignKey)
        {
            List<FieldIndex> indexes = new List<FieldIndex>();

            flatFileDefinitionReference flatFileDefinitionReference = foreignKey.flatFileDefinitionReference;
            foreach (
                recordDefinitionReference recordDefinitionReference in
                flatFileDefinitionReference.recordDefinitionReferences)
            {
                foreach (
                    fieldDefinitionReference fieldDefinitionReference in
                    recordDefinitionReference.fieldDefinitionReferences)
                {
                    indexes.Add(new FieldIndex(flatFileDefinitionReference, recordDefinitionReference,
                        fieldDefinitionReference));
                }
            }

            return indexes;
        }


        private bool IsPartOfPrimaryKey(recordDefinition recordDefinition, fieldDefinition fieldDefinition)
        {
            key[] keys = recordDefinition.keys;
            if (keys != null)
            {
                foreach (key key in keys)
                {
                    fieldDefinitionReference[] keyFieldDefinitionReferences = key.fieldDefinitionReferences;
                    foreach (fieldDefinitionReference fieldDefinitionReference in keyFieldDefinitionReferences)
                    {
                        if (fieldDefinitionReference.name.Equals(fieldDefinition.name))
                        {
                            object o = key.Item;
                            if (o is primaryKey)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private List<recordDefinition> GetRecordDefinitions(flatFileDefinition flatFileDefinition)
        {
            recordDefinition[] recordDefinitions = flatFileDefinition.recordDefinitions;
            Assert.AssertNotNull("recordDefinitions", recordDefinitions);

            return new List<recordDefinition>(recordDefinitions);
        }


        private string GetFileName(string definitionReference)
        {
            flatFile[] flatFiles = GetFlatFiles();
            foreach (flatFile flatFile in flatFiles)
            {
                if (definitionReference.Equals(flatFile.definitionReference))
                {
                    if (flatFile.properties != null)
                    {
                        foreach (property property in flatFile.properties)
                        {
                            if ("fileName".Equals(property.name))
                            {
                                return property.value;
                            }
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
            dataset[] datasets = _addmlInfo.Addml.dataset;
            Assert.AssertNotNull("dataset", datasets);

            if (datasets.Length != 1)
            {
                throw new AddmlDefinitionParseException("dataset must contain exactly one element. Found " +
                                                        datasets.Length +
                                                        " elements");
            }

            return datasets[0];
        }
    }
}