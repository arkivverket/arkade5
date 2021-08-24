using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;
using Serilog;

[assembly: InternalsVisibleTo("Arkivverket.Arkade.Core.Tests")]
namespace Arkivverket.Arkade.Core.Testing.Siard
{
    internal static class SiardValidator
    {
        public static void Validate(string inputFilePath, string reportFilePath)
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

            RunProcess(process);

            ExternalProcessManager.Close(process);
        }

        private static Process SetupSiardValidatorProcess(string fileName, string processArguments)
        {
            var siardValidatorProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = processArguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            return siardValidatorProcess;
        }

        private static void RunProcess(Process process)
        {
            try
            {
                ExternalProcessManager.Start(process);
                process.WaitForExit();
            }
            catch (Exception e)
            {
                Log.Debug(e.ToString());
                throw new ArkadeException(ResolveMessageForException(e));
            }
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
    }
}
