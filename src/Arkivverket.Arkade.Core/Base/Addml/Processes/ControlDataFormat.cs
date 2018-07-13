using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Addml.Definitions.DataTypes;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Testing;
using System.Linq;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Addml.Processes
{
    public class ControlDataFormat : AddmlProcess
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Addml, 19);

        public const string Name = "Control_DataFormat";
        private const int NumberOfShownErrors = 6;

        private readonly Dictionary<FieldIndex, HashSet<string>> _incorrectDataFormat
            = new Dictionary<FieldIndex, HashSet<string>>();

        private readonly List<TestResult> _testResults = new List<TestResult>();

        public override TestId GetId()
        {
            return _id;
        }

        public override string GetName()
        {
            return Name;
        }

        public override string GetDescription()
        {
            return Messages.ControlDataFormatDescription;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentControl;
        }

        protected override List<TestResult> GetTestResults()
        {
            return _testResults;
        }

        protected override void DoRun(FlatFile flatFile)
        {
        }

        protected override void DoRun(Record record)
        {
        }

        protected override void DoEndOfFile()
        {
            foreach (KeyValuePair<FieldIndex, HashSet<string>> entry in _incorrectDataFormat)
            {
                FieldIndex fieldIndex = entry.Key;
                HashSet<string> incorrectDataFormat = entry.Value;

                string errorsToShow = string.Join(", ", incorrectDataFormat
                    .Take(NumberOfShownErrors)
                    .Select(s => "'" + s + "'")
                    .ToList()
                );

                string message = string.Format(Messages.ControlDataFormatMessage, errorsToShow);

                if (incorrectDataFormat.Count > NumberOfShownErrors)
                {
                    int remainingErrors = incorrectDataFormat.Count - NumberOfShownErrors;
                    message += string.Format(Messages.ControlDataFormatMessageExtension, remainingErrors);
                }

                _testResults.Add(new TestResult(ResultType.Error, AddmlLocation.FromFieldIndex(fieldIndex), message));
            }

            _incorrectDataFormat.Clear();
        }

        protected override void DoRun(Field field)
        {
            string value = field.Value;

            DataType dataType = field.Definition.Type;

            if (dataType.IsValid(value))
            {
                return;
            }

            // value is illegal
            FieldIndex fieldIndeks = field.Definition.GetIndex();
            if (!_incorrectDataFormat.ContainsKey(fieldIndeks))
            {
                _incorrectDataFormat.Add(fieldIndeks, new HashSet<string>());
            }

            _incorrectDataFormat[fieldIndeks].Add(value);
        }
    }
}