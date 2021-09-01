using System;
using Launcher.Models;

namespace Launcher.Services
{
    public interface IUpdateInstaller
    {
        event EventHandler<ErrorOccuredEventArgs> OnError; 

        bool ValidateUpdateFileHash(string updateFilePath, string targetFileHash);
        bool TryUnpackUpdateFiles(string updateFilePath);
        bool TryRunUpdater(string updateDirectoryPath, UpdateDescription updateDescription, string processBackBaseArguments);
        void CleanupFiles(string updaterFileName, string updateDirectoryPath);
    }
}