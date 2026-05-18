# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

Arkade 5 is a testing and packaging tool for archival extractions used by Nasjonalarkivet (the Norwegian National Archives). It validates archive extractions against the [ADDML](https://www.arkivverket.no/forvaltning-og-utvikling/regelverk-og-standarder/andre-arkivstandarder/addml-archival-data-description-markup-language) standard and produces information packages (SIP/AIP) based on archival metadata standards. End-user documentation lives at https://arkade.arkivverket.no.

## Build, test, run

The solution targets **.NET 10**. All commands assume `src/` as the working directory.

- Build everything: `dotnet build src/Arkivverket.Arkade.sln`
- Run all tests (xUnit v3): `dotnet test src/Arkivverket.Arkade.sln`
- Run a single test project: `dotnet test src/Arkivverket.Arkade.Core.Tests/Arkivverket.Arkade.Core.Tests.csproj`
- Run a single test by name: `dotnet test src/Arkivverket.Arkade.Core.Tests --filter "FullyQualifiedName~TestClassName.MethodName"`
- Run the CLI in development: `dotnet run --project src/Arkivverket.Arkade.CLI -- <verb> <args>`
- Run the GUI: `dotnet run --project src/Arkivverket.Arkade.GUI` (Windows only — `net10.0-windows` WPF target)

CLI verbs (each supports `--help`): `process`, `test`, `pack`, `generate`, `analyse`, `validate`.

Build artifacts are redirected to `src/artifacts/` via `src/Directory.Build.props`, not the usual per-project `bin/obj`.

The WiX installer project (`Arkivverket.Arkade.GUI.Installer`) builds only on Windows with the WiX toolset.

## Architecture

Three runtime projects plus an installer:

- **Arkivverket.Arkade.Core** — class library (`net10.0`) containing all domain logic: archive parsing, test engines, metadata generation, packaging, format identification. This is the only project that contains business logic.
- **Arkivverket.Arkade.CLI** — console host (`net10.0`) using `CommandLineParser`. Each verb maps to a class in `CLI/Options/`; `Program.cs` dispatches to `CommandLineRunner`, which drives `Core.Base.Arkade`.
- **Arkivverket.Arkade.GUI** — WPF host (`net10.0-windows`) using Prism (Unity container) and MaterialDesignThemes. Standard MVVM split under `Views/` and `ViewModels/`.

### Facade and DI

`Core.Base.Arkade` (in `Core/Base/Arkade.cs`) is the single public entry point both hosts use. It owns an Autofac container built from `ArkadeAutofacModule` (`Core/Util/ArkadeAutofacModule.cs`). The GUI does **not** consume that module — it re-registers the same services in `App.xaml.cs` against Prism's Unity container. **When you add a new injected service to Core, register it in BOTH `ArkadeAutofacModule` (for the CLI/Core API) AND `App.xaml.cs::RegisterTypes` (for the GUI), or one host will fail at resolve time.**

### Test engines

Tests on archive contents are pluggable engines selected by archive type (`TestEngineFactory`):
- ADDML-based archives (Noark 3/4, "fagsystem") → `Core/Base/Addml/AddmlDatasetTestEngine.cs`. ADDML processes implement `IAddmlProcess` and live under `Core/Base/Addml/Processes/`.
- Noark 5 → `Core/Base/Noark5/Noark5TestEngine.cs`. Individual tests live in `Core/Testing/Noark5/` named `N5_NN_TestName.cs` (e.g. `N5_04_NumberOfArchives.cs`). They derive from `Noark5BaseTest`, `Noark5XmlReaderBaseTest`, or `Noark5StructureBaseTest`. New tests are discovered automatically via reflection in `Noark5TestProvider`.
- SIARD (relational DB archives) → `Core/Base/Siard/SiardTestEngine.cs`.

### Processing area

`Core/Base/ArkadeProcessingArea.cs` is a static class that holds the on-disk working directory (logs + temporary extraction) for the current run. Both hosts must call `ArkadeProcessingArea.Establish(...)` before invoking any Core operation — see `Program.cs::ReadyToRun` and `App.xaml.cs` constructor. Internally it uses `AsyncLocal<T>` so it can be configured per logical flow during tests.

### External binaries

`Core/ThirdPartySoftware/` ships platform-specific binaries (Siegfried for PRONOM format identification, with Windows/Linux/Mac builds). The DBPTK jar (`dbptk-app-*.jar`) is intentionally not committed — drop it manually at `Core/ThirdPartySoftware/DBPTK/` for SIARD work. `ExternalProcessManager` (`Core/Util/ExternalProcessManager.cs`) tracks spawned processes and is terminated on shutdown in both hosts.

### Resources and localization

User-facing strings live in `.resx` files under `Core/Resources/` and `GUI/Resources/`. Each has a default (English) and an `*.nb-NO.resx` (Norwegian Bokmål) variant. `Core/Resources/LanguageManager.cs` switches the resource culture for output files; `App.xaml.cs::SetUILanguage` sets the GUI culture from user settings. When adding a user-visible string, add it to both the default and `nb-NO` resx, or output will fall back to English when Norwegian is selected.

### XSDs and external models

Archival schemas (Noark5 versions 3.1/4.0/5.0, ADDML, DIAS METS/PREMIS, EAD3, EAC-CPF, etc.) are embedded as resources in `Core.csproj`. Don't reference them by file path — load via `ResourceUtil` so the CLI and GUI work after publish.

## Testing notes

- Test framework is **xUnit v3** with FluentAssertions and Moq. The shape differs from xUnit 2 (no `[Trait]`s registered through assembly attributes the same way) — if a test attribute behaves oddly, check it's the v3 form.
- Test data fixtures live in `Arkivverket.Arkade.Core.Tests/TestData/`.
- Integration tests under `Arkivverket.Arkade.Core.Tests/Integration/` may exercise the `ArkadeProcessingArea` lifecycle and write to disk; don't parallelize them across project boundaries.
