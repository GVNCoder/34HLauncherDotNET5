using System.IO;

namespace Launcher.Core.Utilities
{
    public static class FileSystemUtility
    {
        // https://stackoverflow.com/questions/278439/creating-a-temporary-directory-in-windows
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
    }
}