using System;
using System.Text;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests;
using Arkivverket.Arkade.Util;
using System.IO;
using System.Linq;

namespace Arkivverket.Arkade.Report
{
    public class HtmlReportGenerator : IReportGenerator
    {
        private readonly StreamWriter _stream;

        public HtmlReportGenerator(StreamWriter stream)
        {
            _stream = stream;
        }

        public void Generate(TestSession testSession)
        {
            StringBuilder html = new StringBuilder();
            _stream.WriteLine(@"<!DOCTYPE html>");
            _stream.WriteLine(@"<html lang=""no"">");
            Head();
            Body(testSession);
            _stream.WriteLine(@"</html>");
            _stream.Flush();
        }

        private void Body(TestSession testSession)
        {
            _stream.WriteLine(@"<body>");
            _stream.WriteLine(@"");
            _stream.WriteLine(@"<div class=""container"">");
            _stream.WriteLine(@"");
            ArkivverketImage();
            _stream.WriteLine(@"    <h1>Testrapport</h1>");
            Summary(testSession);
            _stream.WriteLine(@"    <h2>Tests</h2>");
            foreach (TestRun testRun in testSession.TestSuite.TestRuns)
            {
                Test(testRun);
            }
            _stream.WriteLine(@"</div>");
            _stream.WriteLine(@"</body>");
        }

        private void ArkivverketImage()
        {
            byte[] imageBytes = ResourceUtil.ReadResourceBytes("Arkivverket.Arkade.Resources.arkivverket.gif");
            string imageBase64 = Convert.ToBase64String(imageBytes, Base64FormattingOptions.None);

            _stream.WriteLine(@"<img src=""data:image/gif;base64,");
            _stream.WriteLine(imageBase64);
            _stream.WriteLine(@""" class=""img-responsive"" alt=""Arkivverket"" width=""481"" height=""82"" />");
        }

        private void Test(TestRun testRun)
        {
            _stream.WriteLine(@"    <div class=""test"">");
            _stream.WriteLine(@"        <h3>");
            _stream.WriteLine(@"        " + testRun.TestName);
            _stream.WriteLine(@"        </h3>");
            _stream.WriteLine(@"");
            _stream.WriteLine(@"        <p class=""test-description"">");
            _stream.WriteLine(@"            " + testRun.TestDescription);
            _stream.WriteLine(@"        </p>");
            _stream.WriteLine(@"");
            /*
            sb.AppendLine(@"        <p class=""test-duration"">");
            sb.AppendLine(@"            Tidsbruk: " + testRun.TestDuration + " millisekunder");
            sb.AppendLine(@"        </p>");
            sb.AppendLine(@"");
            */
            _stream.WriteLine(@"        <h4>Testresultater</h4>");
            if (testRun.IsSuccess() && (testRun.TestType == TestType.ContentControl || testRun.TestType == TestType.Structure))
            {
                _stream.WriteLine("<p>Ingen avvik funnet.</p>");
            } 
            else
            {
                _stream.WriteLine(@"        <table class=""table"">");
                _stream.WriteLine(@"            <thead>");
                _stream.WriteLine(@"            <tr>");
                _stream.WriteLine(@"                <th>Lokasjon</th>");
                _stream.WriteLine(@"                <th>Melding</th>");
                _stream.WriteLine(@"            </tr>");
                _stream.WriteLine(@"            </thead>");
                _stream.WriteLine(@"            <tbody>");

                foreach (TestResult testResult in testRun.Results.Take(100)) // TODO only first 100 results are included due to problem loading report in browser
                {
                    _stream.WriteLine(@"            <tr>");
                    _stream.WriteLine(@"                <td>");
                    _stream.WriteLine(@"                " + testResult.Location);
                    _stream.WriteLine(@"                </td>");
                    _stream.WriteLine(@"                <td>");
                    _stream.WriteLine(@"                " + testResult.Message);
                    _stream.WriteLine(@"                </td>");
                    _stream.WriteLine(@"            </tr>");
                }

                _stream.WriteLine(@"            </tbody>");
                _stream.WriteLine(@"        </table>");
            }
            _stream.WriteLine(@"    </div>");
        }

        private void Summary(TestSession testSession)
        {
            _stream.WriteLine(@"    <div class=""summary"">");
            _stream.WriteLine(@"    <div class=""jumbotron"">");
            _stream.WriteLine(@"        <h2>Testsammendrag</h2>");
            _stream.WriteLine(@"");
            _stream.WriteLine(@"        <table class=""table"">");
            _stream.WriteLine(@"            <tbody>");

            _stream.WriteLine(@"            <tr>");
            _stream.WriteLine(@"                <td>");
            _stream.WriteLine(Resources.Report.LabelUuid);
            _stream.WriteLine("                </td>");
            _stream.WriteLine(@"                <td>");
            _stream.WriteLine(testSession.Archive.Uuid);
            _stream.WriteLine("                </td>");
            _stream.WriteLine(@"            </tr>");

            _stream.WriteLine(@"            <tr>");
            _stream.WriteLine(@"                <td>");
            _stream.WriteLine(Resources.Report.LabelArchiveType);
            _stream.WriteLine("                 </td>");
            _stream.WriteLine(@"                <td>");
            _stream.WriteLine(testSession.Archive.ArchiveType);
            _stream.WriteLine("                 </td>");
            _stream.WriteLine(@"            </tr>");

            _stream.WriteLine(@"            <tr>");
            _stream.WriteLine(@"                <td>");
            _stream.WriteLine(Resources.Report.LabelDateOfTesting);
            _stream.WriteLine("                 </td>");
            _stream.WriteLine(@"                <td>");
            _stream.WriteLine(testSession.DateOfTesting.ToString(Resources.Report.DateFormat));
            _stream.WriteLine("                 </td>");
            _stream.WriteLine(@"            </tr>");

            
            _stream.WriteLine(@"            <tr>");
            _stream.WriteLine(@"                <td>");
            _stream.WriteLine(Resources.Report.LabelNumberOfFilesProcessed);
            _stream.WriteLine("                 </td>");
            _stream.WriteLine(@"                <td>");
            _stream.WriteLine(testSession.TestSummary.NumberOfProcessedFiles);
            _stream.WriteLine("                 </td>");
            _stream.WriteLine(@"            </tr>");

            if (testSession.Archive.ArchiveType != ArchiveType.Noark5)
            { 
                _stream.WriteLine(@"            <tr>");
                _stream.WriteLine(@"                <td>");
                _stream.WriteLine(Resources.Report.LabelNumberOfRecordsProcessed);
                _stream.WriteLine("                 </td>");
                _stream.WriteLine(@"                <td>");
                _stream.WriteLine(testSession.TestSummary.NumberOfProcessedRecords);
                _stream.WriteLine("                 </td>");
                _stream.WriteLine(@"            </tr>");
            }

            _stream.WriteLine(@"            <tr>");
            _stream.WriteLine(@"                <td>");
            _stream.WriteLine(Resources.Report.LabelNumberOfErrors);
            _stream.WriteLine("                 </td>");
            _stream.WriteLine(@"                <td>");
            _stream.WriteLine(testSession.TestSuite.FindNumberOfErrors());
            _stream.WriteLine("                 </td>");
            _stream.WriteLine(@"            </tr>");
     
            _stream.WriteLine(@"            </tbody>");
            _stream.WriteLine(@"        </table>");
            _stream.WriteLine(@"    </div>");
            _stream.WriteLine(@"    </div>");
        }

        private void Head()
        {
            _stream.WriteLine(@"<head>");
            _stream.WriteLine(@"    <meta charset=""utf-8"" />");
            _stream.WriteLine(@"    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" />");
            _stream.WriteLine(@"    <meta name=""viewport"" content=""width=device-width, initial-scale=1"" />");
            _stream.WriteLine(@"    <title>Testrapport</title>");
            _stream.WriteLine(@"");
            BootstrapCss();
            ArkadeCss();
            _stream.WriteLine(@"</head>");
        }

        private void ArkadeCss()
        {
            string css = ResourceUtil.ReadResource("Arkivverket.Arkade.Resources.arkade.css");

            _stream.WriteLine(@"<style>");
            _stream.WriteLine(css);
            _stream.WriteLine(@"</style>");
        }

        private void BootstrapCss()
        {
            string css = ResourceUtil.ReadResource("Arkivverket.Arkade.Resources.bootstrap.min.css");

            _stream.WriteLine(@"<style>");
            _stream.WriteLine(css);
            _stream.WriteLine(@"</style>");
        }

    }
}
