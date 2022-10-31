using System;
using System.Globalization;
using System.Windows;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Base.Siard;
using Arkivverket.Arkade.Core.Identify;
using Arkivverket.Arkade.Core.Languages;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Metadata;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Testing.Noark5;
using Arkivverket.Arkade.Core.Testing.Siard;
using Arkivverket.Arkade.GUI.Util;
using Arkivverket.Arkade.Core.Util;
using Arkivverket.Arkade.Core.Util.ArchiveFormatValidation;
using Arkivverket.Arkade.Core.Util.FileFormatIdentification;
using Arkivverket.Arkade.GUI.ViewModels;
using Arkivverket.Arkade.GUI.Views;
using Arkivverket.Arkade.GUI.Languages;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using Serilog;
using Settings = Arkivverket.Arkade.GUI.Properties.Settings;

namespace Arkivverket.Arkade.GUI
{
    public partial class App
    {
        private static ILogger _log;

        public App()
        {
            SetUILanguage();
            
            try
            {
                ArkadeProcessingArea.Establish(Settings.Default.ArkadeProcessingAreaLocation);
            }
            catch (Exception e)
            {
                LogConfiguration.ConfigureSeriLog();
                _log = Log.ForContext<App>();
                _log.Error("Exception while establishing arkade processing area: " + e.Message);
            }

            LogConfiguration.ConfigureSeriLog();
            _log = Log.ForContext<App>();
            // For some reason this will not work for exceptions thrown from inside the Views.
            AppDomain.CurrentDomain.UnhandledException += MyHandler;
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void OnInitialized()
        {
            ApplyUserSettings();

            RegionManager.SetRegionManager(MainWindow, Container.Resolve<IRegionManager>());
            RegionManager.UpdateRegions();
            InitializeModules();

            base.OnInitialized();

            _log.Information("Arkade " + ArkadeVersion.Current + " started");
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            Type moduleAModuleType = typeof(ModuleAModule);
            moduleCatalog.AddModule(new ModuleInfo
            {
                ModuleName = moduleAModuleType.Name,
                ModuleType = moduleAModuleType.AssemblyQualifiedName,
            });
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<TestRunner>();
            containerRegistry.RegisterForNavigation<CreatePackage>();
            containerRegistry.RegisterForNavigation<LoadArchiveExtraction>();

            containerRegistry.Register<AddmlDatasetTestEngine>();
            containerRegistry.Register<AddmlProcessRunner>();
            containerRegistry.Register<IArchiveTypeIdentifier, ArchiveTypeIdentifier>();
            containerRegistry.Register<FlatFileReaderFactory>();
            containerRegistry.Register<Noark5TestEngine>();
            containerRegistry.Register<Noark5TestProvider>();
            containerRegistry.RegisterSingleton<IStatusEventHandler, StatusEventHandler>();
            containerRegistry.Register<ICompressionUtility, TarCompressionUtility>();
            containerRegistry.Register<TestEngineFactory>();
            containerRegistry.Register<ITestProvider, Noark5TestProvider>();
            containerRegistry.Register<TestSessionFactory>();
            containerRegistry.Register<MetadataFilesCreator>();
            containerRegistry.Register<DiasMetsCreator>();
            containerRegistry.Register<DiasPremisCreator>();
            containerRegistry.Register<LogCreator>();
            containerRegistry.Register<EadCreator>();
            containerRegistry.Register<EacCpfCreator>();
            containerRegistry.Register<SubmissionDescriptionCreator>();
            containerRegistry.Register<InformationPackageCreator>();
            containerRegistry.Register<ArkadeApi>();
            containerRegistry.Register<TestSessionXmlGenerator>();
            containerRegistry.Register<IReleaseInfoReader, GitHubReleaseInfoReader>();
            containerRegistry.Register<ArkadeVersion>();
            containerRegistry.Register<IFileFormatInfoFilesGenerator, FileFormatInfoFilesGenerator>();
            containerRegistry.Register<IFileFormatIdentifier, SiegfriedFileFormatIdentifier>();
            containerRegistry.Register<SiegfriedProcessRunner>();
            containerRegistry.Register<ISiardArchiveReader, SiardArchiveReader>();
            containerRegistry.Register<ISiardXmlTableReader, SiardXmlTableReader>();
            containerRegistry.Register<SiardTestEngine>();
            containerRegistry.Register<ISiardValidator, SiardValidator>();
            containerRegistry.Register<SiardMetadataFileHelper>();
            containerRegistry.Register<InformationPackageTypeIdentifier>();
            containerRegistry.RegisterSingleton<ITestProgressReporter, GuiTestProgressReporter>();
            containerRegistry.RegisterSingleton<IArchiveFormatValidator,ArchiveFormatValidator>();
        }

        public void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Container.GetContainer().Dispose();
            var e = (Exception) args.ExceptionObject;
            new DetailedExceptionMessage(e).ShowMessageBox();
            _log.Error("Unexpected exception", e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _log.Information("Arkade " + ArkadeVersion.Current + " stopping");

            ExternalProcessManager.TerminateAll();

            if (!ArkadeProcessingAreaLocationSetting.IsApplied())
                ArkadeProcessingArea.Destroy();

            else if (ArkadeInstance.IsOnlyInstance)
                ArkadeProcessingArea.CleanUp();

            base.OnExit(e);

            Container.GetContainer().Dispose();
        }

        private static void ApplyUserSettings()
        {
            if (Settings.Default.DarkModeEnabled)
                SettingsViewModel.ApplyDarkMode();
            else
                SettingsViewModel.ApplyLightMode();
        }

        private static void SetUILanguage()
        {
            SupportedLanguage uiLanguage = LanguageSettingHelper.GetUILanguage();

            var cultureInfo = CultureInfo.CreateSpecificCulture(uiLanguage.ToString());

            AboutGUI.Culture = cultureInfo;
            CreatePackageGUI.Culture = cultureInfo;
            Languages.GUI.Culture = cultureInfo;
            LoadArchiveExtractionGUI.Culture = cultureInfo;
            MetaDataGUI.Culture = cultureInfo;
            SettingsGUI.Culture = cultureInfo;
            TestRunnerGUI.Culture = cultureInfo;
            ToolsGUI.Culture = cultureInfo;
            TestReportGUI.Culture = cultureInfo;
        }
    }
}
