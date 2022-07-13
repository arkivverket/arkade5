using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;
using Serilog;

namespace Arkivverket.Arkade.Core.Testing.Siard
{
    public class SiardValidator : ISiardValidator
    {
        private readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);
        private readonly IStatusEventHandler _statusEventHandler;
        private readonly ITestProgressReporter _testProgressReporter;

        private readonly string _dbptkLibraryDirectoryPath;

        public SiardValidator(IStatusEventHandler statusEventHandler, ITestProgressReporter testProgressReporter)
        {
            _statusEventHandler = statusEventHandler;
            _testProgressReporter = testProgressReporter;
            _dbptkLibraryDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                ArkadeConstants.DirectoryNameThirdPartySoftware, ArkadeConstants.DirectoryNameDbptk);
        }

        public (List<string>, List<string>) Validate(string inputFilePath, string reportFilePath)
        {
            _statusEventHandler.RaiseEventOperationMessage(Messages.ValidatingExtractMessage, null, OperationMessageStatus.Started);
            _testProgressReporter.Begin(ArchiveType.Siard);

            var dbptkLibraryPath = "";
            try
            {
                dbptkLibraryPath = new DirectoryInfo(_dbptkLibraryDirectoryPath).GetFiles("dbptk-app-2.9.9.jar").First().FullName;
            }
            catch
            {
                _testProgressReporter.Finish(hasFailed: true);
                throw new ArkadeException(
                    string.Format(ExceptionMessages.SiardValidatorLibraryNotFound,
                        Path.GetFileName(dbptkLibraryPath),
                        ArkadeConstants.DbptkLibraryDownloadUrl,
                        _dbptkLibraryDirectoryPath));
            }

            Directory.CreateDirectory(Path.GetDirectoryName(reportFilePath));

            var processArguments = $"-jar \"-Dfile.encoding=UTF-8\" \"{dbptkLibraryPath}\" validate -if \"{inputFilePath}\" -r \"{reportFilePath}\"";

            Process process = SetupSiardValidatorProcess(processArguments);

            (List<string> results, List<string> errors) = RunProcess(process);

            ExternalProcessManager.Close(process);

            CleanUpDbptkLogFiles();

            _statusEventHandler.RaiseEventSiardValidationFinished(errors);

            bool validationRanWithoutRunErrors = errors.All(e => e == null || e.StartsWith("WARN"));

            _testProgressReporter.Finish(hasFailed: !validationRanWithoutRunErrors);

            return (results, errors);
        }

        private void CleanUpDbptkLogFiles()
        {
            try
            {
                Directory.GetFiles(_dbptkLibraryDirectoryPath, "*.txt").ToList().ForEach(File.Delete);
            }
            catch (Exception e)
            {
                _log.Debug("Arkade could not delete log-files from SIARD-validation:\n" + e);
            }
        }

        private static Process SetupSiardValidatorProcess(string processArguments)
        {
            var siardValidatorProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"java",
                    Arguments = processArguments,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            return siardValidatorProcess;
        }

        private (List<string>, List<string>) RunProcess(Process process)
        {
            var results = new List<string>();
            var errors = new List<string>();

            process.OutputDataReceived += (_, args) => results.Add(args.Data);
            process.ErrorDataReceived += (_, args) => errors.Add(args.Data);
            try
            {
                ExternalProcessManager.Start(process);

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();
            }
            catch (Exception e)
            {
                _log.Debug(e.ToString());
                throw new ArkadeException(ResolveMessageForException(e));
            }

            if (errors.Any())
                HandleValidationErrors(errors, results);

            return (results, errors);
        }

        private static string ResolveMessageForException(Exception e)
        {
            return e is Win32Exception
                ? ExceptionMessages.SiardValidatorOpenError
                : ExceptionMessages.SiardValidatorError;
        }

        private static void HandleValidationErrors(IEnumerable<string> errors, ICollection<string> results)
        {
            if (errors.Any(e => e != null && e.Contains("validator only supports: SIARD 2.1 version")))
            {
                results.Clear();
                results.Add(SiardMessages.ErrorMessage);
                results.Add(SiardMessages.ValidatorDoesNotSupportVersionMessage);
            }
        }
    }
}
