using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;
using Serilog;

[assembly: InternalsVisibleTo("Arkivverket.Arkade.Core.Tests")]
namespace Arkivverket.Arkade.Core.Testing.Siard
{
    internal static class SiardValidator
    {
        public static List<string> Validate(string inputFilePath, string reportFilePath)
        {
            if (!File.Exists(BuildDbptkLibraryPath()))
                throw new ArkadeException(
                    string.Format(ExceptionMessages.SiardValidatorLibraryNotFound,
                        Path.GetFileName(BuildDbptkLibraryPath()),
                        ArkadeConstants.SiardValidatorDownloadUrl,
                        AppDomain.CurrentDomain.BaseDirectory));

            Directory.CreateDirectory(Path.GetDirectoryName(reportFilePath));

            const string fileName = @"java";
            
            var processArguments = $"-jar \"-Dfile.encoding=UTF-8\" \"{BuildDbptkLibraryPath()}\" validate -if \"{inputFilePath}\" -r \"{reportFilePath}\"";

            Process process = SetupSiardValidatorProcess(fileName, processArguments);

            List<string> results = RunProcess(process);

            ExternalProcessManager.Close(process);

            return results;
        }

        private static Process SetupSiardValidatorProcess(string fileName, string processArguments)
        {
            var siardValidatorProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
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

        private static List<string> RunProcess(Process process)
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
                Log.Debug(e.ToString());
                throw new ArkadeException(ResolveMessageForException(e));
            }

            if (errors.Any())
                HandleValidationErrors(errors, results);

            return results;
        }

        [Conditional("DEBUG")]
        private static void BeginReadFromProcessOutputForDebug(Process process)
        {
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        private static string BuildDbptkLibraryPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dbptk-app-2.9.9.jar");
        }

        private static string ResolveMessageForException(Exception e)
        {
            return e is Win32Exception
                ? ExceptionMessages.SiardValidatorOpenError
                : ExceptionMessages.SiardValidatorError;
        }

        private static void HandleValidationErrors(List<string> errors, List<string> results)
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
