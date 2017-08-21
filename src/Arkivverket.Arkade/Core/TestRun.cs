using Arkivverket.Arkade.Tests;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arkivverket.Arkade.Core
{

    // TODO:
    // Litt dumt å blande testresultater (Results og TestDuration) med statisk informasjon om testen (resten).
    // Bør splitte dette, slik at det som returneres fra en test-kjøring kun er test-resultater.
    // Selve testen kan legges inn som et interface TestProcess med Name, Type, Category og Description, som også kan brukes for Noark5
    // Trenger ikke eksponere dette interfacet direkte. Kan fint ha gettere som delegerer, slik at dette blir skjult for brukerne av denne klassen

    public class TestRun
    {
        public List<TestResult> Results { get; set; }
        public long TestDuration { get; set; }
        public string TestName { get; set; }
        public TestType TestType { get; set; }
        public string TestDescription { get; set; }

        public TestRun(string testName, TestType testType)
        {
            Results = new List<TestResult>();
            TestName = testName;
            TestType = testType;
        }

        public void Add(TestResult result)
        {
            Results.Add(result);
        }

        public bool IsSuccess()
        {
            var success = true;
            foreach (var result in Results)
            {
                if (result.IsError())
                {
                    success = false;
                    break;
                }
            }
            return success;
        }


        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("Test: ").AppendLine(TestName);
            builder.Append("Test type: ").AppendLine(TestType.ToString());
            builder.Append("IsSuccess: ").AppendLine(IsSuccess().ToString());
            builder.AppendLine("Results: ");

            foreach (var result in Results)
            {
                builder.AppendLine(result.ToString());
            }

            return builder.ToString();
        }

        public int FindNumberOfErrors()
        {
            return Results.Count(r => r.IsError());
        }
    }
}