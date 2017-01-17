using System.Collections.Generic;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;
using System;
using System.Linq;

namespace Arkivverket.Arkade.Tests.Noark5
{    /// <summary>
     ///     Noark5 - test #11
     /// </summary>
    public class NumberOfCaseStatusesPerArchivePart : Noark5XmlReaderBaseTest
    {
        private readonly Dictionary<string, List<CaseStatus>> _saksstatuser = new Dictionary<string, List<CaseStatus>>();

        private string _currentArkivdelName;
        private string _currentArkivdelSystemId;
        private string _currentSaksStatus;
        private int _currentNumberOfSaksstatus;
        List<CaseStatus> _statuses = new List<CaseStatus>();

        public override string GetName()
        {
            return Noark5Messages.NumberOfCaseStatusesPerArchivePart;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("arkivdel"))
            {
                _saksstatuser.Add(_currentArkivdelSystemId, _statuses);

                _currentArkivdelName = null;
                _currentSaksStatus = null;
                _currentNumberOfSaksstatus = 0;
                _statuses = new List<CaseStatus>();
            }
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("tittel", "arkivdel"))
            {
                _currentArkivdelName = eventArgs.Value;
            }
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
            {
                _currentArkivdelSystemId = eventArgs.Value;
            }
            if (eventArgs.Path.Matches("saksstatus", "mappe"))
            {
                if(_currentSaksStatus != eventArgs.Value)
                    _currentNumberOfSaksstatus = 0;

                _currentSaksStatus = eventArgs.Value;
                _currentNumberOfSaksstatus++;
                var status = new CaseStatus { Arkivdel = _currentArkivdelName, Status = _currentSaksStatus, NumberOfSaksstatus = _currentNumberOfSaksstatus };
                if (!_statuses.Contains(status))
                    _statuses.Add(status);
                else
                {
                    var statusCurrent = _statuses.Where(s => s.Status == _currentSaksStatus).First();
                    _statuses.Remove(status);
                    statusCurrent.NumberOfSaksstatus = _currentNumberOfSaksstatus;
                    _statuses.Add(statusCurrent);
                }
            }
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>();
            foreach (var arkivdel in _saksstatuser)
            {
                var statuses = arkivdel.Value;
                foreach (var status in statuses)
                    testResults.Add(new TestResult(ResultType.Success, new Location(""), status.Arkivdel + ", " + status.Status + ": " + status.NumberOfSaksstatus));
            }
            return testResults;
        }

        private class CaseStatus : IEquatable<CaseStatus>
        {
            public string Status;
            public string Arkivdel;
            public int NumberOfSaksstatus;

            public bool Equals(CaseStatus other)
            {
                return this.Status == other.Status;
            }
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }
    }
}