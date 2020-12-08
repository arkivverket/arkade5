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

        private readonly Dictionary<FieldIndex, HashSet<string>> _valuesNotInCodeList
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
            foreach (KeyValuePair<FieldIndex, HashSet<string>> entry in _valuesNotInCodeList)
            {
                FieldIndex fieldIndex = entry.Key;
                HashSet<string> valuesNotInCodeList = entry.Value;

                _testResults.Add(new TestResult(ResultType.Error, AddmlLocation.FromFieldIndex(fieldIndex),
                    string.Format(Messages.ControlCodesMessage, string.Join(" ", valuesNotInCodeList))));
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
            if (!_valuesNotInCodeList.ContainsKey(fieldIndeks))
            {
                _valuesNotInCodeList.Add(fieldIndeks, new HashSet<string>());
            }

            _valuesNotInCodeList[fieldIndeks].Add(value);
        }
    }
}