using System.Text;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Report
{
    public class HtmlReportGenerator : IReportGenerator<HtmlReport>
    {

        public HtmlReportGenerator()
        {
        }

        public HtmlReport Generate(TestSession testSession)
        {
            StringBuilder html = new StringBuilder();
            html.AppendLine(@"<!DOCTYPE html>");
            html.AppendLine(@"<html lang=""no"">");
            html.AppendLine(Head());
            html.AppendLine(Body(testSession));
            html.AppendLine(@"</html>");

            return new HtmlReport(html.ToString());
        }

        private string Body(TestSession testSession)
        {
            var body = new StringBuilder();
            body.AppendLine(@"<body>");
            body.AppendLine(@"");
            body.AppendLine(@"<div class=""container"">");
            body.AppendLine(@"");
            body.AppendLine(@"    <img src=""" + HtmlReport.ARKIVVERKET_IMG + @""" class=""img-responsive"" alt=""Arkivverket"" width=""481"" height=""82""> ");
            body.AppendLine(@"    <h1>Testrapport</h1>");
            body.AppendLine(Summary());
            body.AppendLine(@"    <h2>Tests</h2>");
            foreach (TestRun testRun in testSession.TestSuite.TestRuns)
            {
                body.AppendLine(Test(testRun));
            }
            body.AppendLine(@"</div>");
            body.AppendLine(@"</body>");

            return body.ToString();
        }

        private string Test(TestRun testRun)
        {
            var sb = new StringBuilder(532);
            sb.AppendLine(@"    <div class=""test"">");
            sb.AppendLine(@"        <h3>");
            sb.AppendLine(@"        " + testRun.TestName);
            sb.AppendLine(@"        </h3>");
            sb.AppendLine(@"");
            sb.AppendLine(@"        <p class=""test-description"">");
            sb.AppendLine(@"            " + testRun.TestDescription);
            sb.AppendLine(@"        </p>");
            sb.AppendLine(@"");
            sb.AppendLine(@"        <p class=""test-duration"">");
            sb.AppendLine(@"            Tidsbruk: " + testRun.TestDuration + " millisekunder");
            sb.AppendLine(@"        </p>");
            sb.AppendLine(@"");
            sb.AppendLine(@"        <h4>Feil</h4>");
            sb.AppendLine(@"        <table class=""table"">");
            sb.AppendLine(@"            <thead>");
            sb.AppendLine(@"            <tr>");
            sb.AppendLine(@"                <th>Location</th>");
            sb.AppendLine(@"                <th>Message</th>");
            sb.AppendLine(@"            </tr>");
            sb.AppendLine(@"            </thead>");
            sb.AppendLine(@"            <tbody>");

            foreach (TestResult testResult in testRun.Results)
            {
                sb.AppendLine(@"            <tr>");
                sb.AppendLine(@"                <td>");
                sb.AppendLine(@"                " + testResult.Result);
                sb.AppendLine(@"                </td>");
                sb.AppendLine(@"                <td>");
                sb.AppendLine(@"                " + testResult.Message);
                sb.AppendLine(@"                </td>");
                sb.AppendLine(@"            </tr>");
            }

            sb.AppendLine(@"            </tbody>");
            sb.AppendLine(@"        </table>");
            sb.AppendLine(@"    </div>");

            return sb.ToString();
        }

        private string Summary()
        {
            var summary = new StringBuilder();
            summary.AppendLine(@"    <div class=""summary"">");
            summary.AppendLine(@"    <div class=""jumbotron"">");
            summary.AppendLine(@"        <h2>Testsammendrag</h2>");
            summary.AppendLine(@"");
            summary.AppendLine(@"        <table class=""table"">");
            summary.AppendLine(@"            <thead>");
            summary.AppendLine(@"            <tr>");
            summary.AppendLine(@"                <th>A</th>");
            summary.AppendLine(@"                <th>B</th>");
            summary.AppendLine(@"            </tr>");
            summary.AppendLine(@"            </thead>");
            summary.AppendLine(@"            <tbody>");
            summary.AppendLine(@"            <tr>");
            summary.AppendLine(@"                <td>a</td>");
            summary.AppendLine(@"                <td>b</td>");
            summary.AppendLine(@"            </tr>");
            summary.AppendLine(@"            <tr>");
            summary.AppendLine(@"                <td>a</td>");
            summary.AppendLine(@"                <td>b</td>");
            summary.AppendLine(@"            </tr>");
            summary.AppendLine(@"            </tbody>");
            summary.AppendLine(@"        </table>");
            summary.AppendLine(@"    </div>");
            summary.AppendLine(@"    </div>");

            return summary.ToString();
        }

        private string Head()
        {
            var head = new StringBuilder();
            head.AppendLine(@"<head>");
            head.AppendLine(@"    <meta charset=""utf-8"">");
            head.AppendLine(@"    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">");
            head.AppendLine(@"    <meta name=""viewport"" content=""width=device-width, initial-scale=1"">");
            head.AppendLine(@"    <title>Testrapport</title>");
            head.AppendLine(@"");
            head.AppendLine(@"    <link href=""" + HtmlReport.BOOTSTRAP_CSS + @""" rel=""stylesheet"">");
            head.AppendLine(@"    <link href=""" + HtmlReport.ARKADE_CSS + @""" rel=""stylesheet"">");
            head.AppendLine(@"</head>");
            return head.ToString();
        }
    }
}
