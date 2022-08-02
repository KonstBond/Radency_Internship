global using Task1.Client;
global using System;
using Newtonsoft.Json;


namespace Task1
{
    public class Program
    {
        public static async Task Main()
        {
            try
            {
                DirectoryInfo directoryA = new DirectoryInfo(@"C:\Users\admin\Desktop\folder_a");
                DirectoryInfo directoryB = new DirectoryInfo(@"C:\Users\admin\Desktop\folder_b");
                Core core = new Core(directoryA, directoryB);
                long counter = 0;

                await Task.Run(async () =>
                {
                    while (true)
                    {
                        IEnumerable<ResultClient> client = await core.Transform(await core.Validate(await core.ReadFileAsync()));
                        string result = JsonConvert.SerializeObject(client, Formatting.Indented);
                        if (result == "[]")
                            continue;
                        else
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
                Console.WriteLine("DDDDD");
            }
        }
    }
}
