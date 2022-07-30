using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

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
            await Task.Run(() =>
            {
                while (true)
                {
                    File.AppendAllText(@"C:\Users\admin\Desktop\123.txt", "I'am working");
                    Task.Delay(300);
                }
            });
        }

        protected async override void OnStop()
        {
            await Task.Run(() =>
            {
                File.AppendAllText(@"C:\Users\admin\Desktop\123.txt", "I'am stop" );
            });
        }
    }
}
