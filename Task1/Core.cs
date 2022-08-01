using System;
using System.IO;
using System.Collections.Generic;
using Task1.ClientParser;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Task1.Client;

namespace Task1
{
    public class Core
    {
        private DirectoryInfo _directoryA;
        private DirectoryInfo _directoryB;
        private string ValidatePattern;
        private List<string> CheckFiles;

        public const string QUOTE = "\"";

        public char SeparatorAttribute { get; private set; }
        public string SeparatorAddress { get; private set; }

        public Core(DirectoryInfo directoryA, DirectoryInfo directoryB)
        {
            _directoryA = directoryA;
            _directoryB = directoryB;

            CheckFiles = new List<string>();
        }

        public async Task<IEnumerable<string>> ReadFileAsync()
        {
            FileInfo file = null;
            try
            {
                file = _directoryA.GetFiles(".")
                       .Where(f => f.Name.EndsWith(".txt") || f.Name.EndsWith(".csv"))
                       .Where(f => !CheckFiles.Contains(f.Name))
                       .FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (file == null)
                return Enumerable.Empty<string>();

            if (file.Extension == ".txt")
            {
                SeparatorAttribute = ',';
                SeparatorAddress = @"""";
                ValidatePattern = @"^\w+, \w+, ""\w+, \w+ \d+, \d+"", \d+\.?\d+, \d{4}-\d{2}-\d{2}, \d{7}, \w+";
            }
            else if (file.Extension == ".csv")
            {
                SeparatorAttribute = ';';
                SeparatorAddress = @"""""""";
                ValidatePattern = @"^\w+;\w+;""""""\w+, \w+ \d+, \d+"""""";\d+\.?\d+;\d{4}-\d{2}-\d{2};\d{7};\w+";
            }
            else
                return Enumerable.Empty<string>();

            CheckFiles.Add(file.Name);
            return await ReadAllLinesAsync(file.FullName);
        }

        public async Task<IEnumerable<RawClient>> Validate(IEnumerable<string> rawData)
        {
            IEnumerable<string> validateData = await System.Threading.Tasks.Task.Run<IEnumerable<string>>(() =>
            {
                return from c in rawData
                       where Regex.IsMatch(c, ValidatePattern)
                       select c;
            });

            var context = new Context(new Name(), SeparatorAttribute, SeparatorAddress);
            List<string> clients = validateData.ToList();

            List<RawClient> rawClients = new List<RawClient>();
            for (int i = 0; i < clients.Count; i++)
            {
                RawClient client = new RawClient();
                for (int j = 0; j < 7; j++) //7 различных атрибутов
                {
                    clients[i] = context.RewriteInput(clients[i], client);
                    if (clients[i].StartsWith("ERROR"))
                    {
                        Console.WriteLine(clients[i]);
                        context = new Context(new Name(), SeparatorAttribute, SeparatorAddress);
                        break;
                    }
                }

                rawClients.Add(client);
            }

            return rawClients;
        }

        public async Task<IEnumerable<ResultClient>> Transform(IEnumerable<RawClient> rawClients)
        {
            return await Task.Run(() =>
            {
                var res = from c in rawClients
                          group c by c.Address.Substring(0, c.Address.IndexOf(',')) into city
                          select new ResultClient
                          {
                              City = city.Key,
                              Services = from cS in city
                                         group cS by cS.Service into service
                                         select new Client.Service
                                         {
                                             Name = service.Key,
                                             Payers = from cP in service
                                                      select new Payer
                                                      {
                                                          Name = cP.Name,
                                                          Payment = cP.Payment,
                                                          Date = cP.Date,
                                                          AccountNumber = cP.AccountNumber
                                                      },
                                             Total = service.Where(c => c.Service == service.Key).Sum(c => c.Payment)
                                         },
                              Total = city.Where(c => c.Address.Substring(0, c.Address.IndexOf(',')) == city.Key).Sum(c => c.Payment)
                          };
                return res;
            });
        }

        public async Task WriteFileAsync(string JsonResult, long counter)
        {
            await Task.Run(() =>
            {
                DirectoryInfo directory = Directory.CreateDirectory(_directoryB + $@"/{DateTime.Now.Month}-{DateTime.Now.Day}-{DateTime.Now.Year}");
                File.Create(directory.FullName + $@"output{counter}.json");
                File.WriteAllText(directory.FullName + $@"output{counter}.json", JsonResult, Encoding.UTF8);
            });
        }


        //File.ReadAllLinesAsync / .NET Framework 4.8
        private const int DefaultBufferSize = 4096;
        private const FileOptions DefaultOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;

        private static async Task<IEnumerable<string>> ReadAllLinesAsync(string filePath)
        {
            var lines = new List<string>();

            var sourceStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize, DefaultOptions);
            var reader = new StreamReader(sourceStream, Encoding.Unicode);

            string line;
            while ((line = await reader.ReadLineAsync()) != null)
                lines.Add(line);

            reader.Close();
            sourceStream.Close();
            return lines;
        }
    }

}

