using System.Collections.Generic;
﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Addml.Processes;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Integration
{
    public class Noark3IntegrationTest
    {
        [Fact (Skip = "Unpacked structure looks wrong and test creates testdata \"leftovers\"")]
        public void Test1()
        {
            ArkadeProcessingArea.Establish(Path.Combine(Environment.CurrentDirectory, "TestData"));

            ArchiveFile archive1 =
                ArchiveFile.Read(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\TestData\\tar\\Noark3-eksempel-1\\c3db9d4e-720c-4f75-bfb6-de90231dc44c.tar", ArchiveType.Noark3);

            Arkade.Core.Base.Arkade arkade = new Arkade.Core.Base.Arkade();
            TestSession testSesson = arkade.RunTests(archive1);

            testSesson.Should().NotBeNull();
            TestSuite testSuite = testSesson.TestSuite;
            testSuite.Should().NotBeNull();
            testSuite.TestRuns.Should().NotBeNullOrEmpty();

            List<TestRun> analyseFindMinMaxValues = testSuite.TestRuns
                .Where(run => run.TestName == AnalyseFindMinMaxValues.Name)
                .ToList();
            analyseFindMinMaxValues.Count.Should().Be(1);
        }
    }
}