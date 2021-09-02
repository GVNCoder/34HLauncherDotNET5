// ReSharper disable UnusedAutoPropertyAccessor.Global

using System;

namespace Launcher.Models
{
    public class UpdateDescription
    {
        public string DownloadLink { get; set; }
        public Version LatestVersion { get; set; }
        public string ZipHash { get; set; }
        public string UpdaterFileName { get; set; }
        public string UninstallFileName { get; set; }
        public string UpdateFilesDirectoryName { get; set; }
    }
}