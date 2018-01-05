using System.Collections.Generic;

namespace Arkivverket.Arkade.Util
{
    public class TestId
    {
        public readonly TestKind Kind;
        public readonly uint Number;

        private static readonly Dictionary<TestKind, string> KindPrefix =
            new Dictionary<TestKind, string>
            {
                { TestKind.Noark5, "N5" },
                { TestKind.Addml, "A" },
                { TestKind.AddmlSup, "AS" },
                { TestKind.Other, "O" },
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
            AddmlSup,
            Other,
        }
    }
}
