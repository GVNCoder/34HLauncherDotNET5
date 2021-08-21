using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;

using Launcher.Models;
using Launcher.Services;

using Newtonsoft.Json;

namespace Launcher.Data
{
    public class UpdateInstaller : IUpdateInstaller
    {
        #region IUpdateInstaller

        public event EventHandler<ErrorOccuredEventArgs> OnError;

        public void CleanupFiles(string updaterFileName, string updateDirectoryPath)
        {
            // validate arguments
            if (string.IsNullOrWhiteSpace(updaterFileName) || string.IsNullOrWhiteSpace(updateDirectoryPath))
            {
                _RaiseOnError($"{nameof(CleanupFiles)}", new ArgumentException($"{nameof(updaterFileName)} or {nameof(updateDirectoryPath)}"));
                return;
            }

            // wait for close updater process
            var updaterProcessName = Path.GetFileNameWithoutExtension(updaterFileName);
            var updaterProcess = Process.GetProcessesByName(updaterProcessName)
                .FirstOrDefault();

            if (updaterProcess != null)
            {
                var waitEvent = new ManualResetEvent(false);

                updaterProcess.EnableRaisingEvents = true;
                updaterProcess.Exited += (sender, args) => waitEvent.Set();

                waitEvent.WaitOne();
            }

            // try delete directory
            try
            {
                Directory.Delete(updateDirectoryPath, true);
            }
            catch (Exception exception)
            {
                _RaiseOnError($"{nameof(CleanupFiles)}", exception);
            }
        }

        public bool TryRunUpdater(string updateDirectoryPath, UpdateDescription updateDescription)
        {
            // get current process file path
            var currentProcess = Process.GetCurrentProcess();
            var mainModule = currentProcess.MainModule;
            var launcherBackPath = mainModule?.FileName;

            // build other paths
            var updaterExecutableFilePath = Path.Combine(updateDirectoryPath, updateDescription.UpdaterFileName);
            var updateFilesPath = Path.Combine(updateDirectoryPath, updateDescription.UpdateFilesDirectoryName);

            // build process run arguments
            var processStartArguments = JsonConvert.SerializeObject(new
                { updateFilesDirPath = updateFilesPath, processBackPath = launcherBackPath, deleteListFileName = updateDescription.DeleteListFileName });

            // build process
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = updaterExecutableFilePath,
                    WorkingDirectory = updateDirectoryPath,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    Arguments = processStartArguments,
                    Verb = "runas"
                }
            };

            var runResult = false;

            // try run updater
            try
            {
                runResult = process.Start();
            }
            catch (Exception exception)
            {
                _RaiseOnError("Can't start a new process", exception);
            }

            return runResult;
        }

        public bool TryUnpackUpdateFiles(string updateFilePath)
        {
            try
            {
                var updateDirectory = Path.GetDirectoryName(updateFilePath);
                ZipFile.ExtractToDirectory(updateFilePath, updateDirectory);
            }
            catch (Exception exception)
            {
                _RaiseOnError($"{nameof(TryUnpackUpdateFiles)}", exception);
                return false;
            }

            return true;
        }

        public bool ValidateUpdateFileHash(string updateFilePath, string targetFileHash)
        {
            using (var fileSteam = File.Open(updateFilePath, FileMode.Open, FileAccess.Read))
            using (var md5 = MD5.Create())
            {
                var updateHashBytes = md5.ComputeHash(fileSteam);
                var encodedUpdateHash = BitConverter.ToString(updateHashBytes)
                    .Replace("-", string.Empty)
                    .ToLower();

                return encodedUpdateHash == targetFileHash;
            }
        }

        #endregion

        #region Private helpers

        private void _RaiseOnError(string message, Exception exception = null)
            => OnError?.Invoke(this, new ErrorOccuredEventArgs(message, exception));

        #endregion
    }
}