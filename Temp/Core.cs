using System;
using System.Configuration;

namespace Temp
{
    public class Core
    {

        private DirectoryInfo _directoryA;
        private DirectoryInfo _directoryB;
        public Core()
        {
            var appSettings = ConfigurationManager.AppSettings;
            _directoryA = new DirectoryInfo(appSettings["A"]);
            _directoryB = new DirectoryInfo(appSettings["B"]);
        }
    }
}
