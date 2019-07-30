using System.Collections.Generic;
using System.Numerics;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Addml.Processes
{
    public class A_02_AnalyseCountChars : AddmlProcess
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Addml, 2);

        public const string Name = "Analyse_CountChars";

        private readonly List<TestResult> _testResults = new List<TestResult>();

        private FlatFile _currentFlatFile;
        private BigInteger _numberOfChars;

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
            return Messages.AnalyseCountCharsDescription;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override void DoRun(FlatFile flatFile)
        {
            _numberOfChars = 0;
            _currentFlatFile = flatFile;
        }

        protected override void DoRun(Record record)
        {
        }

        protected override void DoRun(Field field)
        {
            _numberOfChars += field.Value.Length;
        }

        protected override void DoEndOfFile()
        {
            _testResults.Add(new TestResult(ResultType.Success, new Location(_currentFlatFile.Definition.FileName),
                string.Format(Messages.AnalyseCountCharsMessage, _numberOfChars)));
        }

        protected override List<TestResult> GetTestResults()
        {
            return _testResults;
        }
    }
}