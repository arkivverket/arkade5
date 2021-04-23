﻿using System;
using System.IO;
using Arkivverket.Arkade.Core.Base;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Integration
{
    public class Jegerregisteret98IntegrationTest
    {
        [Fact(Skip = "IO-issues")]
        public void ShouldReadSmallVersionOfJegerregisteret98()
        {
            ArkadeProcessingArea.Establish(Path.Combine(Environment.CurrentDirectory, "TestData"));

            ArchiveFile archive = ArchiveFile.Read("..\\..\\TestData\\tar\\jegerregisteret98-small\\20b5f34c-4411-47c3-a0f9-0a8bca631603.tar", ArchiveType.Fagsystem);
            Arkade.Core.Base.Arkade arkade = new Arkade.Core.Base.Arkade();
            TestSession testSesson = arkade.RunTests(archive);

            testSesson.Should().NotBeNull();
            TestSuite testSuite = testSesson.TestSuite;
            testSuite.Should().NotBeNull();
        }

    }
}