﻿using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

using Launcher.Core.Models;
using Launcher.Core.Services;

using Newtonsoft.Json;

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
                { updateFilesPath, processBackPath = launcherBackPath, deleteListFileName = updateDescription.DeleteListFileName });

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