using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Util;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Integration
{
    public class FiskermanntalletAnonIntegrationTest
    {
        [Fact(Skip = "Takes 3 minutes to run")]
        public void ShouldRunTestsOnFiskermanntallet()
        {
            ArchiveFile archive = ArchiveFile.Read("..\\..\\TestData\\tar\\fiskermanntallet-anonymized\\dab6c748-8d1a-4b6d-b091-3a7b8b3cb255.tar", ArchiveType.Noark3);
            Arkade.Core.Base.Arkade arkade = new Arkade.Core.Base.Arkade();
            TestSession testSesson = arkade.RunTests(archive);

            testSesson.Should().NotBeNull();
            TestSuite testSuite = testSesson.TestSuite;
            testSuite.Should().NotBeNull();
            testSuite.TestRuns.Should().NotBeNullOrEmpty();

            var analyseFindMinMaxValueTestId = new TestId(TestId.TestKind.Addml, 9);
            List<TestRun> analyseFindMinMaxValue = testSuite.TestRuns
                .Where(run => run.TestId.Equals(analyseFindMinMaxValueTestId))
                .ToList();
            analyseFindMinMaxValue.Count.Should().Be(1);
        }


        [Fact(Skip = "Was commented out ...")]
        public void AnonymizeFiskermanntallet()
        {
            StreamWriter file = new StreamWriter(@"C:\tmp\file.txt", false, Encodings.ISO_8859_1);      

            Dictionary<string, string> randomFodselsnummerOgNavn = new Dictionary<string, string>();

            string[] allLines = File.ReadAllLines("C:\\tmp\\_testdata\\fiskermanntallet-anon\\manntal2000_2009.dat", Encodings.ISO_8859_1);
            foreach (string line in allLines)
            {
                string[] fields = line.Split(';');

                // Anonymize birth number
                string fnr = fields[1];
                string randomFnr = NorwegianBirthNumber.CreateRandom(fnr).ToString();

                fields[1] = randomFnr;

                // Anonymize name
                string randomName;
                if (randomFodselsnummerOgNavn.ContainsKey(randomFnr))
                {
                    randomName = randomFodselsnummerOgNavn[randomFnr];
                }
                else
                {
                    randomName = NorwegianNameGenerator.RandomLastNameAndFirstName().ToUpper();
                    randomFodselsnummerOgNavn.Add(randomFnr, randomName);
                }
                fields[2] = randomName;

                // Anonymize address
                fields[3] = fields[3].Length > 0 ? "PORTV. 2" : "";

                file.WriteLine(string.Join(";", fields));
            }

            file.Close();
        }
    }
}