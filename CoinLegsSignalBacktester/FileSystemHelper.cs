using System.Runtime.InteropServices;

namespace CoinLegsSignalBacktester
{
    namespace CoinLegsSignalDataCollector
    {
        public static class FileSystemHelper
        {
            public static string GetBaseDirectory()
            {
                var path = AppDomain.CurrentDomain.BaseDirectory;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    path = Directory.GetCurrentDirectory();
                }

                return path;
            }
        }
    }
}