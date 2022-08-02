using System;
using System.IO;
using System.Collections.Generic;
using Task1.ClientParser;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Task1.Client;
using Task1.Logger;

namespace Task1
{
    public class Core
    {
        private DirectoryInfo _directoryA;
        private DirectoryInfo _directoryB;
        private DirectoryInfo _directoryC;
        private string ValidatePattern;
        private List<string> CheckFiles;
        private DateTime today;
        private MetaInfo _metaInfo;

        public const string QUOTE = "\"";

        public char SeparatorAttribute { get; private set; }
        public string SeparatorAddress { get; private set; }

        public Core(DirectoryInfo directoryA, DirectoryInfo directoryB)
        {
            _directoryA = directoryA;
            _directoryB = directoryB;
            today = DateTime.Now;
            _metaInfo = new MetaInfo();
            CheckFiles = new List<string>();
        }

        public async Task<List<string>> ReadFileAsync()
        {
            FileInfo file = _directoryA.GetFiles(".")
                   .Where(f => f.Extension == ".txt" || f.Extension == ".csv")
                   .Where(f => !CheckFiles.Contains(f.Name))
                   .FirstOrDefault();

            if (file == null)
                return Enumerable.Empty<string>() as List<string>;

            if (file.Extension == ".txt")
            {
                SeparatorAttribute = ',';
                SeparatorAddress = @"""";
                ValidatePattern = @"^\w+, \w+, ""\w+, \w+ \d+, \d+"",  \d+\.?\d+, \d{4}-\d{2}-\d{2}, \d{7}, \w+";
            }
            else if (file.Extension == ".csv")
            {
                SeparatorAttribute = ';';
                SeparatorAddress = @"""""""";
                ValidatePattern = @"^\w+;\w+;""""""\w+, \w+ \d+, \d+"""""";\d+\.?\d+;\d{4}-\d{2}-\d{2};\d{7};\w+";
            }
            else
                return Enumerable.Empty<string>() as List<string>;

            _metaInfo.ParsedFiles++;
            CheckFiles.Add(file.Name);

            List<string> rawData = await ReadAllLinesAsync(file.FullName) as List<string>;

            IEnumerable<string> validateData = await Task.Run(() =>
            {
                return from c in rawData
                       where Regex.IsMatch(c, ValidatePattern)
                       select c;
            });

            List<string> clients = validateData.ToList();

            _metaInfo.ParsedLines = rawData.Count();
            if (rawData.Count() != clients.Count)
            {
                _metaInfo.FoundErrors = rawData.Count() - clients.Count;
                _metaInfo.InvalidFiles.Add(file.FullName);
            }

            return clients;
        }

        public List<RawClient> TransformToClient(List<string> clients)
        {
            if (clients is null || clients.Count == 0)
                return Enumerable.Empty<RawClient>() as List<RawClient>;

            var context = new Context(new Name(), SeparatorAttribute, SeparatorAddress);

            List <RawClient> rawClients = new List<RawClient>();
            for (int i = 0; i < clients.Count(); i++)
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

        public async Task<List<ResultClient>> Transform(List<RawClient> rawClients)
        {
            if (rawClients is null || rawClients.Count == 0)
                return Enumerable.Empty<ResultClient>() as List<ResultClient>;

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
                                                          Date = cP.Date.Year + "-" + cP.Date.Day +  "-" + cP.Date.Month,
                                                          AccountNumber = cP.AccountNumber
                                                      },
                                             Total = service.Where(c => c.Service == service.Key).Sum(c => c.Payment)
                                         },
                              Total = city.Where(c => c.Address.Substring(0, c.Address.IndexOf(',')) == city.Key).Sum(c => c.Payment)
                          };
                return res.ToList();
            });
            
        }

        public void WriteFile(string JsonResult, long counter)
        {
            DirectoryInfo directory = Directory.CreateDirectory(_directoryB + $@"\{today.Month}-{today.Day}-{today.Year}\");
            _directoryC = directory;
            StreamWriter writer = File.CreateText(_directoryC.FullName + $"output{counter}.json");
            writer.Write(JsonResult);
            writer.Close();
        }

        public void NewDay()
        { 
            if (!File.Exists(_directoryC.FullName + "meta.log"))
            {
                StreamWriter writer = File.CreateText(_directoryC.FullName + $"meta.log");
                writer.Write($"parsed files: {_metaInfo.ParsedFiles}\n" +
                    $"parsed lines: {_metaInfo.ParsedLines}\n" +
                    $"found errors: {_metaInfo.FoundErrors}\n" +
                    $"invalid files: {JsonConvert.SerializeObject(_metaInfo.InvalidFiles)}");
                writer.Close();

                _metaInfo = new MetaInfo();
                today = today.AddDays(1);
            }
        }


        //File.ReadAllLinesAsync / .NET Framework 4.8
        private const int DefaultBufferSize = 4096;
        private const FileOptions DefaultOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;

        private static async Task<IEnumerable<string>> ReadAllLinesAsync(string filePath)
        {
            var lines = new List<string>();

            var sourceStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize, DefaultOptions);
            var reader = new StreamReader(sourceStream, Encoding.UTF8);

            string line;
            while ((line = await reader.ReadLineAsync()) != null)
                lines.Add(line);

            reader.Close();
            sourceStream.Close();
            return lines;
        }
    }

}

