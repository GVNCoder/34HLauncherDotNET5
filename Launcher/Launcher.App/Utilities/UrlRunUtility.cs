using System.Diagnostics;

namespace Launcher.Utilities
{
    // https://brockallen.com/2016/09/24/process-start-for-urls-on-net-core/
    public static class UrlRunUtility
    {
        public static void Open(string url)
            => Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
    }
}