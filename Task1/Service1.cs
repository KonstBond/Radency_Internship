using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Task1.Client;
using System.IO;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Configuration;

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
            try
            {
                DirectoryInfo directoryA = new DirectoryInfo(ConfigurationManager.AppSettings["A"]);
                DirectoryInfo directoryB = new DirectoryInfo(ConfigurationManager.AppSettings["B"]);
                Core core = new Core(directoryA, directoryB);
                long counter = 0;

                await Task.Run(async () =>
                {
                    while (true)
                    {
                        IEnumerable<ResultClient> client = await core.Transform(await core.Validate(await core.ReadFileAsync()));
                        string result = JsonConvert.SerializeObject(client, Formatting.Indented);

                        await core.WriteFileAsync(result, ++counter);
                    }
                });

            }
            catch (Exception ex)
            {
                StreamWriter streamWriter = File.CreateText(@"C:\Users\admin\Desktop\123.txt");
                await streamWriter.WriteAsync(ex.Message + "\n");
                streamWriter.Close();
            }
            finally
            {
                OnStop();
            }
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
