using System;
using System.Configuration;
using System.IO;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows;

namespace Task1
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;

            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            if (!Directory.Exists(ConfigurationManager.AppSettings["A"]))
                MessageBox.Show($"Directory A does not exist + {ConfigurationManager.AppSettings["A"]}", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            else if (!Directory.Exists(ConfigurationManager.AppSettings["B"]))
                MessageBox.Show("Directory B does not exist", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
                ServiceBase.Run(ServicesToRun);
        }
    }
}
