using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Arkivverket.Arkade.Logging;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core.Noark5
{
    public class Noark5TestEngine : ITestEngine
    {
        private readonly IArchiveContentReader _archiveContentReader;
        private readonly ITestProvider _testProvider;
        private readonly IStatusEventHandler _statusEventHandler;

        public Noark5TestEngine(IArchiveContentReader archiveContentReader, ITestProvider testProvider, IStatusEventHandler statusEventHandler)
        {
            _archiveContentReader = archiveContentReader;
            _testProvider = testProvider;
            _statusEventHandler = statusEventHandler;
        }

        public event EventHandler<ReadElementEventArgs> ReadStartElementEvent;
        public event EventHandler<ReadElementEventArgs> ReadElementValueEvent;
        public event EventHandler<ReadElementEventArgs> ReadEndElementEvent;


        public TestSuite RunTestsOnArchive(TestSession testSession)
        {
            List<ITest> testsForArchive = _testProvider.GetTestsForArchive(testSession.Archive);

            SubscribeTestsToReadElementEvent(testsForArchive);

            using (var reader = XmlReader.Create(_archiveContentReader.GetContentAsStream(testSession.Archive)))
            {
                RaiseEventStartParsingFile();

                var path = new Stack<string>();

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            path.Push(reader.LocalName);
                            RaiseReadStartElementEvent(CreateReadElementEventArgs(reader, path));
                            break;
                        case XmlNodeType.Text:
                            RaiseReadElementValueEvent(CreateReadElementEventArgs(reader, path));
                            break;
                        case XmlNodeType.EndElement:
                            path.Pop();
                            RaiseReadEndElementEvent(CreateReadElementEventArgs(reader, path));
                            _statusEventHandler.RaiseEventRecordProcessingStopped();
                            break;
                        case XmlNodeType.XmlDeclaration:
                        case XmlNodeType.ProcessingInstruction:
                        case XmlNodeType.Comment:
                        case XmlNodeType.Whitespace:
                            break;
                    }
                }
                RaiseEventFinishedParsingFile();

            }

            return CreateTestSuiteFromResults(testsForArchive);
        }

        private void RaiseEventStartParsingFile()
        {
            _statusEventHandler.RaiseEventFileProcessingStarted(
                new FileProcessingStatusEventArgs(Archive.ContentDescriptionFileNameNoark5, Archive.ContentDescriptionFileNameNoark5));
        }

        private void RaiseEventFinishedParsingFile()
        {
            _statusEventHandler.RaiseEventFileProcessingFinished(
                new FileProcessingStatusEventArgs(Archive.ContentDescriptionFileNameNoark5, Archive.ContentDescriptionFileNameNoark5, true));
        }

        private static ReadElementEventArgs CreateReadElementEventArgs(XmlReader reader, Stack<string> path)
        {
            return new ReadElementEventArgs(reader.Name, reader.Value, new ElementPath(path.ToList()));
        }

        private static TestSuite CreateTestSuiteFromResults(List<ITest> testsForArchive)
        {
            var testSuite = new TestSuite();
            foreach (var test in testsForArchive)
                testSuite.AddTestRun(test.GetTestRun());

            return testSuite;
        }

        private void SubscribeTestsToReadElementEvent(List<ITest> testsForArchive)
        {
            foreach (var test in testsForArchive)
            {
                ReadStartElementEvent += test.OnReadStartElementEvent;
                ReadElementValueEvent += test.OnReadElementValueEvent;
                ReadEndElementEvent += test.OnReadEndElementEvent;
            }
        }

        private void RaiseReadStartElementEvent(ReadElementEventArgs readElementEventArgs)
        {
            var handler = ReadStartElementEvent;
            handler?.Invoke(this, readElementEventArgs);
        }
        private void RaiseReadElementValueEvent(ReadElementEventArgs readElementEventArgs)
        {
            var handler = ReadElementValueEvent;
            handler?.Invoke(this, readElementEventArgs);
        }

        private void RaiseReadEndElementEvent(ReadElementEventArgs readElementEventArgs)
        {
            var handler = ReadEndElementEvent;
            handler?.Invoke(this, readElementEventArgs);
        }
    }
}
