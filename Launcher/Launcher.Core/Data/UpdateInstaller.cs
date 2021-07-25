using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

using Launcher.Core.Models;
using Launcher.Core.Services;

namespace Launcher.Core.Data
{
    public class UpdateInstaller : IUpdateInstaller
    {
        #region IUpdateInstaller

        public event EventHandler<ErrorOccuredEventArgs> OnError;

        public void CleanupFiles(string updatesDirectory)
        {
            throw new NotImplementedException();
        }

        public bool TryRunUpdater(string updaterFilePath, string updateFilesDirectoryName)
        {
            var runResult = false;
            var launcherBackPath = Process.GetCurrentProcess().MainModule?.FileName;
            var updateDirectory = Path.GetDirectoryName(updaterFilePath);
            var updateFilesPath = Path.Combine(updateDirectory, updateFilesDirectoryName);

            var process = new Process();
            var processRunInfo = new ProcessStartInfo
            {
                FileName = updaterFilePath,
                CreateNoWindow = true,
                UseShellExecute = false,
                Arguments = string.Join('|', $"updateFilesPath={updateFilesPath}", $"processBackPath={launcherBackPath}"),
                Verb = "runas"
            };

            // apply run settings
            process.StartInfo = processRunInfo;

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
            var isErrorOccured = false;

            try
            {
                var updateDirectory = Path.GetDirectoryName(updateFilePath);
                ZipFile.ExtractToDirectory(updateFilePath, updateDirectory);
            }
            catch (Exception exception)
            {
                _RaiseOnError($"{nameof(TryUnpackUpdateFiles)}", exception);
                isErrorOccured = true;
            }

            return isErrorOccured;
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