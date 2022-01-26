using System.Collections.Generic;
using System.Collections.Immutable;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Addml.Definitions.DataTypes;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Testing;
using System.Linq;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Addml.Processes
{
    public class A_19_ControlDataFormat : AddmlProcess
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Addml, 19);

        public const string Name = "Control_DataFormat";
        private const int NumberOfShownErrors = 6;

        private readonly Dictionary<FieldIndex, Dictionary<string, HashSet<long>>> _incorrectDataFormat = new();

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
            foreach ((FieldIndex fieldIndex, Dictionary<string, HashSet<long>> incorrectDataFormat) in _incorrectDataFormat)
            {
                string errorsToShow = string.Join(", ", incorrectDataFormat.Keys
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

                IEnumerable<long> recordNumbers = incorrectDataFormat.SelectMany(p => p.Value).ToImmutableSortedSet();
                _testResults.Add(new TestResult(ResultType.Error,
                    new Location(AddmlLocation.FromFieldIndex(fieldIndex).ToString(), recordNumbers), message));
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
            FieldIndex fieldIndex = field.Definition.GetIndex();
            if (_incorrectDataFormat.ContainsKey(fieldIndex))
            {
                if (_incorrectDataFormat[fieldIndex].ContainsKey(value))
                    _incorrectDataFormat[fieldIndex][value].Add(CurrentRecordNumber);
                
                else
                    _incorrectDataFormat[fieldIndex].Add(value, new HashSet<long> { CurrentRecordNumber });
            }
            else
            {
                _incorrectDataFormat.Add(fieldIndex,
                    new Dictionary<string, HashSet<long>>
                    {
                        { value, new HashSet<long> { CurrentRecordNumber } }
                    });
            }
        }
    }
}