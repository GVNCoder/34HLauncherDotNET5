// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InvertIf
// ReSharper disable RedundantAssignment
// ReSharper disable InlineOutVariableDeclaration

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

using Newtonsoft.Json;
using Updater.App.Models;

namespace Updater.App
{
    public class Updater
    {
        #region Constants

        private const int RunArgumentsIndex = 1;

        #endregion

        private readonly string[] _deleteFiles;
        private readonly string _destinationDirectoryPath;
        private readonly UpdaterRunArguments _runArguments;

        #region Ctor

        private Updater(UpdaterRunArguments runArguments)
        {
            _runArguments = runArguments;

            // populate internal state
            _destinationDirectoryPath = Path.GetDirectoryName(runArguments.ProcessBackPath);
            _deleteFiles = File.ReadAllLines(runArguments.DeleteListFileName)
                .Where(s => string.IsNullOrWhiteSpace(s) == false)
                .Select(relativePath => Path.Combine(_destinationDirectoryPath, relativePath))
                .ToArray();
        }

        #endregion

        public static void Main(string[] args)
        {
            // get run arguments
            var rawArguments = args[RunArgumentsIndex];
            var runArguments = JsonConvert.DeserializeObject<UpdaterRunArguments>(rawArguments);

            // create an application instance
            var updater = new Updater(runArguments);
            var errorMessage = string.Empty;

            // step 1 Close process and wait it
            updater.CloseAndWaitTargetProcess();

            // step 2 Verify delete file list
            updater.VerifyFilesAvailability(out errorMessage);
            if (string.IsNullOrEmpty(errorMessage) == false)
            {
                updater.FinalizeUpdater(errorMessage);
            }

            // step 3 Delete files
            updater.DeleteFiles();

            // step 4 Move files
            updater.MoveFiles();

            // step 5 Run process back
            updater.FinalizeUpdater(errorMessage);
        }

        private void CloseAndWaitTargetProcess()
        {
            var targetProcessName = Path.GetFileNameWithoutExtension(_runArguments.ProcessBackPath);
            var process = Process.GetProcessesByName(targetProcessName)
                .FirstOrDefault();

            if (process != null)
            {
                var waitForProcessExistEvent = new ManualResetEvent(false);

                process.EnableRaisingEvents = true;
                process.Exited += (sender, eventArgs) => waitForProcessExistEvent.Set();

                // try to close process
                process.CloseMainWindow();

                waitForProcessExistEvent.WaitOne();
            }
        }

        private void VerifyFilesAvailability(out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                foreach (var filePath in _deleteFiles)
                {
                    var file = new FileInfo(filePath);
                    if (file.Exists)
                    {
                        // check file access
                        using (var _ = file.OpenWrite())
                        {
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                errorMessage = $"{nameof(VerifyFilesAvailability)} exception {exception.GetType().Name}. Message {exception.Message}";
            }
        }

        private void DeleteFiles()
        {
            foreach (var filePath in _deleteFiles)
            {
                File.Delete(filePath);
            }
        }

        private void MoveFiles()
        {
            var sourceDirectoryFiles =
                Directory.EnumerateFiles(_runArguments.UpdateFilesDirPath, "*.*", SearchOption.TopDirectoryOnly);

            foreach (var sourceDirectoryFile in sourceDirectoryFiles)
            {
                Directory.Move(sourceDirectoryFile, _destinationDirectoryPath);
            }
        }

        private void FinalizeUpdater(string errorMessage)
        {
            // run process back
            var currentAppDomain = AppDomain.CurrentDomain;
            var currentWorkingDirectory = currentAppDomain.BaseDirectory;
            var currentExecutablePath = Path.Combine(currentWorkingDirectory, currentAppDomain.FriendlyName);
            var workingDirectory = _destinationDirectoryPath;
            var argumentValue = JsonConvert.SerializeObject(new
                { updaterFileName = currentExecutablePath, updateDirPath = currentWorkingDirectory, errorMessage });

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _runArguments.ProcessBackPath,
                    WorkingDirectory = workingDirectory,
                    Arguments = $"{_runArguments.ProcessBackBaseArguments} -postUpdDesc={argumentValue}"
                }
            };

            process.Start();

            // stop current execution
            Environment.Exit(0);
        }
    }
}