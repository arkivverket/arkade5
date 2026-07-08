// Archive domain types were relocated to Arkivverket.Arkade.Core.Base.Archives during
// the Core renewal; test code references them pervasively.
global using Arkivverket.Arkade.Core.Base.Archives;

// xUnit v3 introduced Xunit.TestResult, which collides with Arkade's own
// Arkivverket.Arkade.Core.Testing.TestResult throughout the test suite.
global using TestResult = Arkivverket.Arkade.Core.Testing.TestResult;
