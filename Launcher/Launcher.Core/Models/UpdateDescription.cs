using System;

namespace Launcher.Core.Models
{
    public class UpdateDescription
    {
        public string DownloadLink { get; set; }
        public Version LatestVersion { get; set; }
        public string ZipHash { get; set; }
    }
}