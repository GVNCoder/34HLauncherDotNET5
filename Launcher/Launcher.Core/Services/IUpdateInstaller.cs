using System;
using Launcher.Core.Models;

namespace Launcher.Core.Services
{
    public interface IUpdateInstaller
    {
        event EventHandler<ErrorOccuredEventArgs> OnError; 

        bool ValidateUpdateFileHash(string updateFilePath, string targetFileHash);
        bool TryUnpackUpdateFiles(string updateFilePath);
        bool TryRunUpdater(string updateDirectoryPath, UpdateDescription updateDescription);
        void CleanupFiles(string updateDirectory);
    }
}