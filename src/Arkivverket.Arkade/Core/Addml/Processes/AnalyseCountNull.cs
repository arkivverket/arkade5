using System.Collections.Generic;
using System.Numerics;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Tests;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class AnalyseCountNull : AddmlProcess
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Addml, 7);

        public const string Name = "Analyse_CountNULL";

        private readonly Dictionary<FieldIndex, BigInteger> _nullCount
            = new Dictionary<FieldIndex, BigInteger>();

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
            return Messages.AnalyseCountNullDescription;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
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

        protected override void DoRun(Field field)
        {
            string value = field.Value;
            bool isNull = field.Definition.Type.IsNull(value);

            FieldIndex index = field.Definition.GetIndex();
            if (!_nullCount.ContainsKey(index))
            {
                _nullCount.Add(index, new BigInteger(0L));
            }

            if (isNull)
            {
                _nullCount[index]++;
            }
        }

        protected override void DoEndOfFile()
        {
            foreach (KeyValuePair<FieldIndex, BigInteger> entry in _nullCount)
            {
                FieldIndex index = entry.Key;
                BigInteger nullCount = entry.Value;

                _testResults.Add(new TestResult(ResultType.Success, AddmlLocation.FromFieldIndex(index),
                    string.Format(Messages.AnalyseCountNullMessage, nullCount)));
            }

            _nullCount.Clear();
        }
    }
}