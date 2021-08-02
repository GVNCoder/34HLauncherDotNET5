using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Updater.App.Models;

namespace Updater.App
{
    public static class Updater
    {
        private const int RunArgumentsIndex = 1;

        public static void Main(string[] args)
        {
            // get run arguments
            var rawArguments = args[RunArgumentsIndex];
            var arguments = JsonConvert.DeserializeObject<UpdaterRunArguments>(rawArguments);

            // close launcher
            var processName = Path.GetFileNameWithoutExtension(arguments.ProcessBackPath);
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

            // verify access to file and remove it
            var filesToDelete = File.ReadAllLines(arguments.DeleteListFileName)
                .Where(s => string.IsNullOrWhiteSpace(s) == false)
                .ToArray();

            try
            {
                foreach (var filePath in filesToDelete)
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
                FinalizeUpdater(arguments.ProcessBackPath, $"Delete list exception type {exception.GetType().Name}. Message {exception.Message}");
            }

            foreach (var filePath in filesToDelete)
            {
                File.Delete(filePath);
            }

            //var filesToMove = Directory.GetFiles(arguments.UpdateFilesDirPath, "*.*", SearchOption.TopDirectoryOnly);
            var moveToDir = Path.GetDirectoryName(arguments.ProcessBackPath);

            Directory.Move(arguments.UpdateFilesDirPath, moveToDir);

            FinalizeUpdater(arguments.ProcessBackPath, null);
        }

        #region Private helpers

        private static void FinalizeUpdater(string processBackPath, string errorMessage)
        {
            // run process back
            var currentProcess = Process.GetCurrentProcess();
            var mainModule = currentProcess.MainModule;
            var currentExecutablePath = mainModule?.FileName;
            var currentWorkingDirectory = Path.GetDirectoryName(currentExecutablePath);

            var workingDirectory = Path.GetDirectoryName(processBackPath);
            var arguments = JsonConvert.SerializeObject(new
                { updaterFileName = currentExecutablePath, updateDirPath = currentWorkingDirectory, errorMessage });

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = processBackPath,
                    WorkingDirectory = workingDirectory,
                    Arguments = arguments
                }
            };

            process.Start();

            // stop current execution
            Environment.Exit(0);
        }

        #endregion
    }
}