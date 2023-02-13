using System.Collections.Generic;
using System.Numerics;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Addml.Processes
{
    public class A_07_AnalyseCountNull : AddmlProcess
    {
        private readonly TestId _id = new(TestId.TestKind.Addml, 7);

        public const string Name = "Analyse_CountNULL";

        private readonly Dictionary<FieldIndex, NullCount> _nullCount = new();

        private readonly List<TestResult> _testResults = new();

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
            bool isValidNullValue = field.Definition.Type.IsValidNullValue(value);

            FieldIndex index = field.Definition.GetIndex();

            if (isValidNullValue)
            {
                if (!_nullCount.ContainsKey(index))
                {
                    _nullCount.Add(index, new NullCount { ValidNullCount = new BigInteger(1L) });
                    return;
                }
                _nullCount[index].ValidNullCount++;
                return;
            }

            if (!string.IsNullOrWhiteSpace(value))
                return;

            if (!_nullCount.ContainsKey(index))
            {
                _nullCount.Add(index, new NullCount { InvalidNullCount = new BigInteger(1L) });
                return;
            }
            _nullCount[index].InvalidNullCount++;
        }

        protected override void DoEndOfFile()
        {
            foreach ((FieldIndex index, NullCount nullCount) in _nullCount)
            {
                _testResults.Add(new TestResult(ResultType.Success, AddmlLocation.FromFieldIndex(index),
                    string.Format(Messages.AnalyseCountNullMessage, nullCount.TotalNullCount, nullCount.ValidNullCount,
                        nullCount.InvalidNullCount)));
            }

            _nullCount.Clear();
        }

        private class NullCount
        {
            public BigInteger ValidNullCount { get; set; }
            public BigInteger InvalidNullCount { get; set; }
            public BigInteger TotalNullCount => ValidNullCount + InvalidNullCount;

            public NullCount()
            {
                ValidNullCount = BigInteger.Zero;
                InvalidNullCount = BigInteger.Zero;
            }
        }
    }
}