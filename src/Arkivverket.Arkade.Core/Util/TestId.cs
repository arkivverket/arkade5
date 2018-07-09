using System.Collections.Generic;
﻿using System;

namespace Arkivverket.Arkade.Core.Util
{
    public class TestId : IComparable
    {
        public readonly TestKind Kind;
        public readonly uint Number;

        private static readonly Dictionary<TestKind, string> KindPrefix =
            new Dictionary<TestKind, string>
            {
                { TestKind.Noark5, "N5" },
                { TestKind.Addml, "A" },
                { TestKind.AddmlHardcoded, "AH" },
                { TestKind.AddmlInternal, "AI" },
                { TestKind.Unidentified, "U" },
            };

        public TestId(TestKind testKind, uint number)
        {
            Kind = testKind;
            Number = number;
        }

        public override string ToString()
        {
            return $"{KindPrefix[Kind]}.{Number:D2}";
        }

        public enum TestKind
        {
            Noark5,
            Addml,
            AddmlHardcoded,
            AddmlInternal,
            Unidentified
        }

        public int CompareTo(object obj)
        {
            var testId = (TestId) obj;

            int kindComparison = Kind.CompareTo(testId.Kind);
            int numberComparison = Number.CompareTo(testId.Number);

            return kindComparison == 0 ? numberComparison : kindComparison;
        }

        public override bool Equals(object obj)
        {
            var testId = (TestId) obj;

            return Kind == testId?.Kind && Number == testId.Number;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (int) Kind * 397 ^ (int) Number;
            }
        }
    }
}
