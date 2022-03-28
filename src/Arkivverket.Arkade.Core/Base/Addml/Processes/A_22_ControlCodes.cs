using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Addml.Processes
{
    public class A_22_ControlCodes : AddmlProcess
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Addml, 22);

        public const string Name = "Control_Codes";

        private readonly Dictionary<FieldIndex, Dictionary<string, HashSet<long>>> _valuesNotInCodeList = new();

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
            return Messages.ControlCodesDescription;
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
            foreach ((FieldIndex fieldIndex, Dictionary<string, HashSet<long>> recordNumbersPerValueNotInCodeList)in _valuesNotInCodeList)
            {
                foreach ((string valueNotInCodeList, HashSet<long> recordNumbers) in recordNumbersPerValueNotInCodeList)
                {
                    _testResults.Add(new TestResult(ResultType.Error,
                        new Location(AddmlLocation.FromFieldIndex(fieldIndex).ToString(), recordNumbers),
                        string.Format(Messages.ControlCodesMessage, valueNotInCodeList)));
                }
            }

            _valuesNotInCodeList.Clear();
        }

        protected override void DoRun(Field field)
        {
            string value = field.Value;

            List<AddmlCode> codes = field.Definition.Codes;
            if (codes == null)
            {
                return;
            }

            foreach (AddmlCode addmlCode in codes)
            {
                if (addmlCode.GetCodeValue() == value)
                {
                    return;
                }
            }

            // value is not in code list
            FieldIndex fieldIndeks = field.Definition.GetIndex();
            if (_valuesNotInCodeList.ContainsKey(fieldIndeks))
            {
                if (_valuesNotInCodeList[fieldIndeks].ContainsKey(value))
                    _valuesNotInCodeList[fieldIndeks][value].Add(CurrentRecordNumber);
                else
                    _valuesNotInCodeList[fieldIndeks].Add(value, new HashSet<long>{CurrentRecordNumber});
            }
            else
            {
                _valuesNotInCodeList.Add(fieldIndeks, new Dictionary<string, HashSet<long>>
                {
                    {value, new HashSet<long>{CurrentRecordNumber}}
                });
            }
        }
    }
}