using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class ControlForeignKey : IAddmlProcess
    {
        private const string Name = "Control_ForeignKey";

        private readonly Dictionary<string, HashSet<string>> _primaryKeys = new Dictionary<string, HashSet<string>>();

        public string GetName()
        {
            return Name;
        }

        public void Run(Field field)
        {
            // TODO: IsPartOfPrimaryKey is probably not correct here!
            if (field.Definition.IsPartOfPrimaryKey())
            {
                string file = field.Definition.GetAddmlFlatFileDefinition().Name;
                string key = file + "_" + field.Definition.Name;

                HashSet<string> primaryKeysForField = null;
                if (_primaryKeys.ContainsKey(key))
                {
                    primaryKeysForField = _primaryKeys[key];
                }
                else
                {
                    primaryKeysForField = new HashSet<string>();
                    _primaryKeys.Add(key, primaryKeysForField);
                }
                primaryKeysForField.Add(field.Value);
            }
            if (field.Definition.ForeignKey != null)
            {
                string foreignKeyReferenceFieldName = field.Definition.ForeignKey.Name;
                string foreignKeyReferenceFileName = field.Definition.ForeignKey.GetAddmlFlatFileDefinition().Name;
                // save foreign key reference. cannot be sure that the file it references has been parsed yet.
            }
        }

        public void Run(FlatFile flatFile)
        {
        }

        public void Run(Record record)
        {
        }

        public TestRun GetTestRun()
        {
            throw new System.NotImplementedException();
        }

        public void EndOfFile()
        {
            throw new System.NotImplementedException();
        }
    }
}