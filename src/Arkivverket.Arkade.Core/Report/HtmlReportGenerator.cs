using System;
using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Util;
using System.IO;
using System.Linq;

namespace Arkivverket.Arkade.Core.Report
{
    public class HtmlReportGenerator : IReportGenerator
    {
        private static int _testResultDisplayLimit;
        private static ArchiveType _archiveType;

        public HtmlReportGenerator(int testResultDisplayLimit)
        {
            _testResultDisplayLimit = testResultDisplayLimit;
        }

        public void Generate(TestReport testReport, Stream stream)
        {
            _archiveType = testReport.Summary.ArchiveType;
            var streamWriter = new StreamWriter(stream);
            streamWriter.WriteLine(@"<!DOCTYPE html>");
            streamWriter.WriteLine(@"<html lang=""no"">");
            Head(testReport.Summary.Uuid, streamWriter); // NB! UUID-writeout
            Body(testReport, streamWriter);
            streamWriter.WriteLine(@"</html>");
            streamWriter.Flush();
        }

        private static void Body(TestReport testReport, StreamWriter stream)
        {
            stream.WriteLine(@"<body>");
            stream.WriteLine(@"");
            stream.WriteLine(@"<div class=""container"">");
            stream.WriteLine(@"");
            ArkivverketImage(stream);
            stream.WriteLine(@"    <h1>" + Resources.Report.HeadingTestReport + "</h1>");
            Summary(testReport, stream);

            if (!_archiveType.Equals(ArchiveType.Siard))
            {
                SummaryOfErrors(testReport, stream);
                stream.WriteLine(@"    <h2>" + Resources.Report.HeadingTests + "</h2>");
            }

            foreach (ExecutedTest test in testReport.TestsResults)
            {
                Test(test, stream);
            }
            stream.WriteLine(@"<hr/>");
            VersionNumber(stream);
            stream.WriteLine(@"</div>");
            stream.WriteLine(@"</body>");
        }

        private static void VersionNumber(StreamWriter stream)
        {
            stream.WriteLine(@"<p class=""text-right"">");
            stream.WriteLine(Resources.Report.FooterArkadeVersion, ArkadeVersion.Current);
            stream.WriteLine("</p>");
        }

        private static void ArkivverketImage(StreamWriter stream)
        {
            byte[] imageBytes = ResourceUtil.ReadResourceBytes("Arkivverket.Arkade.Core.Resources.arkivverket.gif");
            string imageBase64 = Convert.ToBase64String(imageBytes, Base64FormattingOptions.None);

            stream.WriteLine(@"<img src=""data:image/gif;base64,");
            stream.WriteLine(imageBase64);
            stream.WriteLine(@""" class=""img-responsive"" alt=""Arkivverket"" width=""481"" height=""82"" />");
        }

        private static void Test(ExecutedTest test, StreamWriter stream)
        {
            stream.WriteLine(@"    <div class=""test"">");
            stream.WriteLine($@"       <h3 id=""{test.TestId}"">");
            stream.WriteLine(@"        " + test.TestName);
            stream.WriteLine(@"        </h3>");
            if (!_archiveType.Equals(ArchiveType.Siard))
            {
                stream.WriteLine(@"        <p><b>Type:</b> " + GetTestTypeDisplayName(test.TestType) + "</p>");
                stream.WriteLine(@"");
                stream.WriteLine(@"        <p class=""test-description"">");
                stream.WriteLine($@"            {test.TestDescription}", "<br>");
                stream.WriteLine(@"        </p>");
                stream.WriteLine(@"");
                stream.WriteLine(@"        <h4>" + Resources.Report.HeadingTestResults + "</h4>");
            }

            if (TestTypeIsControl(test) && !test.HasResults)
            {
                stream.WriteLine("<p>" + Resources.Report.TestNoErrorsFound + "</p>");
            }
            else
            {
                stream.WriteLine(@"        <table class=""table"" style=""width: 100%"">");
                SetTableColumnsWidth(stream);
                stream.WriteLine(@"            <thead>");
                stream.WriteLine(@"            <tr>");
                stream.WriteLine(@"                <th>" + Resources.Report.TestLocation + "</th>");
                stream.WriteLine(@"                <th>" + Resources.Report.TestMessage + "</th>");
                stream.WriteLine(@"            </tr>");
                stream.WriteLine(@"            </thead>");
                stream.WriteLine(@"            <tbody>");

                WriteResults(test.ResultSet, stream);

                stream.WriteLine(@"            </tbody>");
                stream.WriteLine(@"        </table>");
            }

            stream.WriteLine(@"    </div>");
        }

        private static void SetTableColumnsWidth(StreamWriter stream)
        {
            stream.WriteLine(@"            <colgroup>");
            stream.WriteLine(@"                <col span=""1"" style=""width: 20%;"">");
            stream.WriteLine(@"                <col span=""1"" style=""width: 80%;"">");
            stream.WriteLine(@"            </colgroup>");
        }

        private static void WriteResults(ResultSet resultSet, TextWriter stream, int level = 0)
        {
            switch (level)
            {
                case 0:
                    break;
                case 1:
                    stream.WriteLine(@"            <tr>");
                    stream.WriteLine(@"                <td colspan='2'><b><i>" + resultSet.Name + "</i></b></td>");
                    stream.WriteLine(@"            </tr>");
                    break;
                default:
                    stream.WriteLine(@"            <tr>");
                    stream.WriteLine(@"                <td colspan='2'>");
                    stream.WriteLine(@"                    <p style='margin-left: 40px'>");
                    stream.WriteLine(@"                        <b><i>" + resultSet.Name + "</i></b>");
                    stream.WriteLine(@"                    </p>");
                    stream.WriteLine(@"                </td>");
                    stream.WriteLine(@"            </tr>");
                    break;
            }

            IEnumerable<Result> resultsToDisplay = _testResultDisplayLimit == -1
                ? resultSet.Results
                : resultSet.Results.Take(_testResultDisplayLimit);

            foreach (Result result in resultsToDisplay)
            {
                stream.WriteLine(@"            <tr>");
                if (_archiveType.Equals(ArchiveType.Siard))
                    stream.WriteLine(@"                <td><a href='" + result.Location.String + "'>" + result.Location.String + "</a></td>");
                else
                    stream.WriteLine(@"                <td>" + result.Location.String + "</td>");
                stream.WriteLine(@"                <td>");
                stream.WriteLine(@"                " + SubstituteLineBreaksWithHtmlBreak(result.Message));
                stream.WriteLine(@"                </td>");
                stream.WriteLine(@"            </tr>");
            }

            if (_testResultDisplayLimit > 0 && resultSet.Results.Count > _testResultDisplayLimit)
            {
                string moreResultsMessage = string.Format(
                    Resources.Report.TestMoreResultsOfSameKind,
                    resultSet.Results.Count - _testResultDisplayLimit
                );

                stream.WriteLine(@"            <tr>");
                stream.WriteLine(@"                <td></td>");
                stream.WriteLine(@"                <td>" + moreResultsMessage + "</td>");
                stream.WriteLine(@"            </tr>");
            }

            level++;
            foreach (ResultSet subResultSet in resultSet.ResultSets)
            {
                WriteResults(subResultSet, stream, level);
            }
        }

        private static bool TestTypeIsControl(ExecutedTest test)
        {
            return (test.TestType is TestType.ContentControl or TestType.StructureControl);
        }

        private static string GetTestTypeDisplayName(TestType? testType)
        {
            return testType switch
            {
                TestType.ContentAnalysis => Resources.Report.TestTypeContentAnalysisDisplayName,
                TestType.ContentControl => Resources.Report.TestTypeContentControlDisplayName,
                TestType.StructureAnalysis => Resources.Report.TestTypeStructureAnalysisDisplayName,
                TestType.StructureControl => Resources.Report.TestTypeStructureControlDisplayName,
                _ => testType.ToString()
            };
        }

        private static string SubstituteLineBreaksWithHtmlBreak(string input)
        {
            return input.Replace("\n", "<br/>");
        }

        private static void Summary(TestReport testReport, StreamWriter stream)
        {
            stream.WriteLine(@"    <div class=""summary"">");
            stream.WriteLine(@"    <div class=""jumbotron"">");
            stream.WriteLine(@"        <h2>" + Resources.Report.HeadingTestSummary + "</h2>");
            stream.WriteLine(@"");
            stream.WriteLine(@"        <table class=""table"">");
            stream.WriteLine(@"            <tbody>");

            stream.WriteLine(@"            <tr>");
            stream.WriteLine(@"                <td>");
            stream.WriteLine(Resources.Report.LabelUuid);
            stream.WriteLine("                </td>");
            stream.WriteLine(@"                <td>");
            stream.WriteLine(testReport.Summary.Uuid); // NB! UUID-writeout
            stream.WriteLine("                </td>");
            stream.WriteLine(@"            </tr>");
            
            stream.WriteLine(@"            <tr>");
            stream.WriteLine(@"                <td>");
            stream.WriteLine(Resources.Report.LabelArchiveCreators);
            stream.WriteLine("                </td>");
            stream.WriteLine(@"                <td>");
            stream.WriteLine(testReport.Summary.ArchiveCreators);
            stream.WriteLine("                </td>");
            stream.WriteLine(@"            </tr>");

            stream.WriteLine(@"            <tr>");
            stream.WriteLine(@"                <td>");
            stream.WriteLine(Resources.Report.LabelArchivePeriod);
            stream.WriteLine("                </td>");
            stream.WriteLine(@"                <td>");
            stream.WriteLine(testReport.Summary.ArchivalPeriod);
            stream.WriteLine("                </td>");
            stream.WriteLine(@"            </tr>");

            stream.WriteLine(@"            <tr>");
            stream.WriteLine(@"                <td>");
            stream.WriteLine(Resources.Report.LabelSystemName);
            stream.WriteLine("                </td>");
            stream.WriteLine(@"                <td>");
            stream.WriteLine(testReport.Summary.SystemName);
            stream.WriteLine("                </td>");
            stream.WriteLine(@"            </tr>");

            stream.WriteLine(@"            <tr>");
            stream.WriteLine(@"                <td>");
            stream.WriteLine(Resources.Report.LabelSystemType);
            stream.WriteLine("                </td>");
            stream.WriteLine(@"                <td>");
            stream.WriteLine(testReport.Summary.SystemType);
            stream.WriteLine("                </td>");
            stream.WriteLine(@"            </tr>");

            stream.WriteLine(@"            <tr>");
            stream.WriteLine(@"                <td>");
            stream.WriteLine(Resources.Report.LabelArchiveType);
            stream.WriteLine("                </td>");
            stream.WriteLine(@"                <td>");
            stream.WriteLine(testReport.Summary.ArchiveType.ToString());
            stream.WriteLine("                </td>");
            stream.WriteLine(@"            </tr>");

            stream.WriteLine(@"            <tr>");
            stream.WriteLine(@"                <td>");
            stream.WriteLine(Resources.Report.LabelDateOfTesting);
            stream.WriteLine("                </td>");
            stream.WriteLine(@"                <td>");
            stream.WriteLine(testReport.Summary.DateOfTesting);
            stream.WriteLine("                </td>");
            stream.WriteLine(@"            </tr>");

            if (_archiveType != ArchiveType.Siard)
            {
                if (_archiveType == ArchiveType.Noark5)
                {
                    stream.WriteLine(@"            <tr>");
                    stream.WriteLine(@"                <td>");
                    stream.WriteLine(Resources.Report.LabelNumberOfTestsExecuted);
                    stream.WriteLine("                </td>");
                    stream.WriteLine(@"                <td>");
                    stream.WriteLine(testReport.Summary.NumberOfTestsRun);
                    stream.WriteLine("                </td>");
                    stream.WriteLine(@"            </tr>");
                }

                if (testReport.Summary.NumberOfProcessedFiles != default && _archiveType != ArchiveType.Noark5)
                {
                    stream.WriteLine(@"            <tr>");
                    stream.WriteLine(@"                <td>");
                    stream.WriteLine(Resources.Report.LabelNumberOfFilesProcessed);
                    stream.WriteLine("                </td>");
                    stream.WriteLine(@"                <td>");
                    stream.WriteLine(testReport.Summary.NumberOfProcessedFiles);
                    stream.WriteLine("                </td>");
                    stream.WriteLine(@"            </tr>");
                }

                if (testReport.Summary.NumberOfProcessedRecords != default && _archiveType != ArchiveType.Noark5)
                {
                    stream.WriteLine(@"            <tr>");
                    stream.WriteLine(@"                <td>");
                    stream.WriteLine(Resources.Report.LabelNumberOfRecordsProcessed);
                    stream.WriteLine("                </td>");
                    stream.WriteLine(@"                <td>");
                    stream.WriteLine(testReport.Summary.NumberOfProcessedRecords);
                    stream.WriteLine("                </td>");
                    stream.WriteLine(@"            </tr>");
                }
            }

            stream.WriteLine(@"            <tr>");
            stream.WriteLine(@"                <td>");
            stream.WriteLine(Resources.Report.LabelNumberOfErrors);
            stream.WriteLine("                </td>");
            stream.WriteLine(@"                <td>");
            stream.WriteLine(testReport.Summary.NumberOfErrors);
            stream.WriteLine("                </td>");
            stream.WriteLine(@"            </tr>");

            if (_archiveType is ArchiveType.Siard)
            {
                stream.WriteLine(@"            <tr>");
                stream.WriteLine(@"                <td>");
                stream.WriteLine(Resources.Report.LabelNumberOfWarnings);
                stream.WriteLine("                </td>");
                stream.WriteLine(@"                <td>");
                stream.WriteLine(testReport.Summary.NumberOfWarnings);
                stream.WriteLine("                </td>");
                stream.WriteLine(@"            </tr>");
            }
            stream.WriteLine(@"            </tbody>");
            stream.WriteLine(@"        </table>");
            stream.WriteLine(@"    </div>");
            stream.WriteLine(@"    </div>");
        }

        private static void SummaryOfErrors(TestReport testReport, StreamWriter stream)
        {
            stream.WriteLine(@"    <div class=""summary"">");
            stream.WriteLine(@"    <div class=""jumbotron"">");
            stream.WriteLine(@"        <h2>" + Resources.Report.HeadingDeviations + "</h2>");
            stream.WriteLine(@"");
            stream.WriteLine(@"        <table class=""table"">");
            stream.WriteLine(@"            <tbody>");

            foreach (ExecutedTest test in testReport.TestsResults)
            {
                if (int.Parse(test.NumberOfErrors) > 0)
                {
                    stream.WriteLine(@"            <tr>");
                    stream.WriteLine(@"                <td>");
                    stream.WriteLine(@"<a href=""#" + test.TestId + @""">" + test.TestName + @"</a>");
                    stream.WriteLine(@"                </td>");
                    stream.WriteLine(@"                <td>");
                    stream.WriteLine(test.NumberOfErrors);
                    stream.WriteLine(@"                </td>");
                    stream.WriteLine(@"            </tr>");
                }
            }
            stream.WriteLine(@"            </tbody>");
            stream.WriteLine(@"        </table>");
            stream.WriteLine(@"    </div>");
            stream.WriteLine(@"    </div>");
        }

        private static void Head(string title, StreamWriter stream)
        {
            stream.WriteLine(@"<head>");
            stream.WriteLine(@"    <meta charset=""utf-8"" />");
            stream.WriteLine(@"    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" />");
            stream.WriteLine(@"    <meta name=""viewport"" content=""width=device-width, initial-scale=1"" />");
            stream.WriteLine(@"    <title>" + title + "</title>");
            stream.WriteLine(@"");
            BootstrapCss(stream);
            ArkadeCss(stream);
            stream.WriteLine(@"</head>");
        }

        private static void ArkadeCss(StreamWriter stream)
        {
            string css = ResourceUtil.ReadResource("Arkivverket.Arkade.Core.Resources.arkade.css");

            stream.WriteLine(@"<style>");
            stream.WriteLine(css);
            stream.WriteLine(@"</style>");
        }

        private static void BootstrapCss(StreamWriter stream)
        {
            string css = ResourceUtil.ReadResource("Arkivverket.Arkade.Core.Resources.bootstrap.min.css");

            stream.WriteLine(@"<style>");
            stream.WriteLine(css);
            stream.WriteLine(@"</style>");
        }
    }
}
