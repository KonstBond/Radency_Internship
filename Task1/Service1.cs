using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Task1.Client;
using System.IO;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Configuration;
using System.Diagnostics;
using System.Windows;

namespace Task1
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected async override void OnStart(string[] args)
        {
            DirectoryInfo directoryA = new DirectoryInfo(ConfigurationManager.AppSettings["A"]);
            DirectoryInfo directoryB = new DirectoryInfo(ConfigurationManager.AppSettings["B"]);
            Core core = new Core(directoryA, directoryB);
            long counter = 0;
            bool isNewday = true;

            await Task.Run(async () =>
            {
                while (true)
                {
                    if (DateTime.Now.Hour == 21 && DateTime.Now.Minute == 12 && !isNewday)
                        isNewday = true;

                    if (DateTime.Now.Hour == 21 && DateTime.Now.Minute == 11 && DateTime.Now.Second >= 0 && isNewday)
                    {
                        core.NewDay();
                        counter = 0;
                        isNewday = false;
                    }

                    List<ResultClient> clients = await core.Transform(core.TransformToClient(await core.ReadFileAsync()));
                    if (clients is null)
                        continue;
                    string result = JsonConvert.SerializeObject(clients, Formatting.Indented);
                    if (result == "[]")
                        continue;
                    else
                    {
                        core.WriteFile(result, ++counter);
                    }
                }
            });
        }

        protected async override void OnStop()
        {
            await Task.Run(async () =>
            {
                StreamWriter writer = File.CreateText(@"C:\Users\admin\Desktop\123.txt");
                await writer.WriteAsync("Iam Stop my work");
                writer.Close();
            });
        }
    }
}
