using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace UpdateTool
{
    public static class Tool
    {
        private const string SourceDirectoryName = "Source";
        private static readonly string DestinationFileName = Path.Combine("Output", "update.zip");

        public static void  Main(string[] args)
        {
            ZipFile.CreateFromDirectory(SourceDirectoryName, DestinationFileName, CompressionLevel.Optimal, false);

            using (var fileSteam = File.Open(DestinationFileName, FileMode.Open, FileAccess.Read))
            using (var md5 = MD5.Create())
            {
                var updateHashBytes = md5.ComputeHash(fileSteam);
                var encodedUpdateHash = BitConverter.ToString(updateHashBytes)
                    .Replace("-", string.Empty)
                    .ToLower();

                // report
                Console.WriteLine($"Output hash: {encodedUpdateHash}");
            }

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
