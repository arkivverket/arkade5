using System;
using System.Text;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Report
{
    public class HtmlReportGenerator : IReportGenerator
    {
        IReport IReportGenerator.Generate(TestSession testSession)
        {
            return Generate(testSession);
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
            body.AppendLine(ArkivverketImage());
            body.AppendLine(@"    <h1>Testrapport</h1>");
            body.AppendLine(Summary(testSession));
            body.AppendLine(@"    <h2>Tests</h2>");
            foreach (TestRun testRun in testSession.TestSuite.TestRuns)
            {
                body.AppendLine(Test(testRun));
            }
            body.AppendLine(@"</div>");
            body.AppendLine(@"</body>");

            return body.ToString();
        }

        private string ArkivverketImage()
        {
            byte[] imageBytes = ResourceUtil.ReadResourceBytes("Arkivverket.Arkade.Resources.arkivverket.gif");
            string imageBase64 = Convert.ToBase64String(imageBytes, Base64FormattingOptions.None);

            var image = new StringBuilder();
            image.Append(@"<img src=""data:image/gif;base64,");
            image.Append(imageBase64);
            image.Append(@""" class=""img-responsive"" alt=""Arkivverket"" width=""481"" height=""82"" />");
            return image.ToString();
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
            /*
            sb.AppendLine(@"        <p class=""test-duration"">");
            sb.AppendLine(@"            Tidsbruk: " + testRun.TestDuration + " millisekunder");
            sb.AppendLine(@"        </p>");
            sb.AppendLine(@"");
            */
            sb.AppendLine(@"        <h4>Testresultater</h4>");
            if (testRun.IsSuccess() && testRun.TestType == TestType.ContentControl || testRun.TestType == TestType.Structure)
            {
                sb.AppendLine("<p>Ingen avvik funnet.</p>");
            } 
            else
            {
                sb.AppendLine(@"        <table class=""table"">");
                sb.AppendLine(@"            <thead>");
                sb.AppendLine(@"            <tr>");
                sb.AppendLine(@"                <th>Lokasjon</th>");
                sb.AppendLine(@"                <th>Melding</th>");
                sb.AppendLine(@"            </tr>");
                sb.AppendLine(@"            </thead>");
                sb.AppendLine(@"            <tbody>");

                foreach (TestResult testResult in testRun.Results)
                {
                    sb.AppendLine(@"            <tr>");
                    sb.AppendLine(@"                <td>");
                    sb.AppendLine(@"                " + testResult.Location);
                    sb.AppendLine(@"                </td>");
                    sb.AppendLine(@"                <td>");
                    sb.AppendLine(@"                " + testResult.Message);
                    sb.AppendLine(@"                </td>");
                    sb.AppendLine(@"            </tr>");
                }

                sb.AppendLine(@"            </tbody>");
                sb.AppendLine(@"        </table>");
                sb.AppendLine(@"    </div>");
            }
            return sb.ToString();
        }

        private string Summary(TestSession testSession)
        {
            var summary = new StringBuilder();
            summary.AppendLine(@"    <div class=""summary"">");
            summary.AppendLine(@"    <div class=""jumbotron"">");
            summary.AppendLine(@"        <h2>Testsammendrag</h2>");
            summary.AppendLine(@"");
            summary.AppendLine(@"        <table class=""table"">");
            summary.AppendLine(@"            <tbody>");

            summary.AppendLine(@"            <tr>");
            summary.Append(@"                <td>").Append(Resources.Report.LabelUuid).AppendLine("</td>");
            summary.Append(@"                <td>").Append(testSession.Archive.Uuid).AppendLine("</td>");
            summary.AppendLine(@"            </tr>");

            summary.AppendLine(@"            <tr>");
            summary.Append(@"                <td>").Append(Resources.Report.LabelArchiveType).AppendLine("</td>");
            summary.Append(@"                <td>").Append(testSession.Archive.ArchiveType).AppendLine("</td>");
            summary.AppendLine(@"            </tr>");

            summary.AppendLine(@"            <tr>");
            summary.Append(@"                <td>").Append(Resources.Report.LabelDateOfTesting).AppendLine("</td>");
            summary.Append(@"                <td>").Append(testSession.DateOfTesting.ToString(Resources.Report.DateFormat)).AppendLine("</td>");
            summary.AppendLine(@"            </tr>");

            
            summary.AppendLine(@"            <tr>");
            summary.Append(@"                <td>").Append(Resources.Report.LabelNumberOfFilesProcessed).AppendLine("</td>");
            summary.Append(@"                <td>").Append(testSession.TestSummary.NumberOfProcessedFiles).AppendLine("</td>");
            summary.AppendLine(@"            </tr>");

            if (testSession.Archive.ArchiveType != ArchiveType.Noark5)
            { 
                summary.AppendLine(@"            <tr>");
                summary.Append(@"                <td>").Append(Resources.Report.LabelNumberOfRecordsProcessed).AppendLine("</td>");
                summary.Append(@"                <td>").Append(testSession.TestSummary.NumberOfProcessedRecords).AppendLine("</td>");
                summary.AppendLine(@"            </tr>");
            }

            summary.AppendLine(@"            <tr>");
            summary.Append(@"                <td>").Append(Resources.Report.LabelNumberOfErrors).AppendLine("</td>");
            summary.Append(@"                <td>").Append(testSession.TestSuite.FindNumberOfErrors()).AppendLine("</td>");
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
            head.AppendLine(@"    <meta charset=""utf-8"" />");
            head.AppendLine(@"    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" />");
            head.AppendLine(@"    <meta name=""viewport"" content=""width=device-width, initial-scale=1"" />");
            head.AppendLine(@"    <title>Testrapport</title>");
            head.AppendLine(@"");
            head.AppendLine(BootstrapCss());
            head.AppendLine(ArkadeCss());
            head.AppendLine(@"</head>");
            return head.ToString();
        }

        private string ArkadeCss()
        {
            string css = ResourceUtil.ReadResource("Arkivverket.Arkade.Resources.arkade.css");

            var sb = new StringBuilder();
            sb.AppendLine(@"<style>");
            sb.AppendLine(css);
            sb.AppendLine(@"</style>");
            return sb.ToString();
        }

        private string BootstrapCss()
        {
            string css = ResourceUtil.ReadResource("Arkivverket.Arkade.Resources.bootstrap.min.css");

            var sb = new StringBuilder();
            sb.AppendLine(@"<style>");
            sb.AppendLine(css);
            sb.AppendLine(@"</style>");
            return sb.ToString();
        }

    }
}
