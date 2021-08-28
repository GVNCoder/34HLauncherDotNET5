using System;
using System.IO;
using System.Security;

namespace Launcher.Utilities
{
    public static class FileSystemUtility
    {
        // https://stackoverflow.com/questions/278439/creating-a-temporary-directory-in-windows
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public static string GetTemporaryDirectory()
        {
            // generate temp directory path
            var tempPath = Path.GetTempPath();
            var tempFileName = Path.GetRandomFileName();
            var tempDirectory = Path.Combine(tempPath, tempFileName);

            // generate physical directory
            Directory.CreateDirectory(tempDirectory);

            return tempDirectory;
        }

        public static bool FileExists(string path)
            => File.Exists(path);
    }
}