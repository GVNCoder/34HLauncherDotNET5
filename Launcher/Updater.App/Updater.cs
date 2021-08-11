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

        private readonly UpdaterRunArguments _runArguments;
        private readonly string[] _deleteFiles;

        #region Ctor

        private Updater(UpdaterRunArguments runArguments)
        {
            _runArguments = runArguments;
            _deleteFiles = File.ReadAllLines(_runArguments.DeleteListFileName)
                .Where(s => string.IsNullOrWhiteSpace(s) == false)
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
            var processName = Path.GetFileNameWithoutExtension(_runArguments.ProcessBackPath);
            var process = Process.GetProcessesByName(processName)
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
            var sourceDirectory = _runArguments.UpdateFilesDirPath;
            var destinationDirectory = Path.GetDirectoryName(_runArguments.ProcessBackPath);
            var sourceDirectoryFiles =
                Directory.EnumerateFiles(sourceDirectory, "*.*", SearchOption.TopDirectoryOnly);

            foreach (var sourceDirectoryFile in sourceDirectoryFiles)
            {
                Directory.Move(sourceDirectoryFile, destinationDirectory);
            }
        }

        private void FinalizeUpdater(string errorMessage)
        {
            var currentAppDomain = AppDomain.CurrentDomain;
            var currentWorkingDirectory = currentAppDomain.BaseDirectory;
            var currentExecutablePath = Path.Combine(currentWorkingDirectory, currentAppDomain.FriendlyName);

            // run process back
            var workingDirectory = Path.GetDirectoryName(_runArguments.ProcessBackPath);
            var arguments = JsonConvert.SerializeObject(new
                { updaterFileName = currentExecutablePath, updateDirPath = currentWorkingDirectory, errorMessage });

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _runArguments.ProcessBackPath,
                    WorkingDirectory = workingDirectory,
                    Arguments = arguments
                }
            };

            process.Start();

            // stop current execution
            Environment.Exit(0);
        }
    }
}