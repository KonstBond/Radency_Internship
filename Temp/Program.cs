global using Task1.Client;
global using System;
using Newtonsoft.Json;


namespace Task1
{
    public class Program
    {
        public static async Task Main()
        {
            Core core = new Core();

            while (true)
            {
                await System.Threading.Tasks.Task.Run((Func<System.Threading.Tasks.Task?>)(async () =>
                {
                    IEnumerable<ResultClient> client = await core.Transform(await core.Validate(await core.ReadFileAsync()));
                    string result = JsonConvert.SerializeObject(client, Formatting.Indented);             
                }));
            }
        }
    }
}
