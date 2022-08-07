using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Task2;
using Task2.Database;

public class Program
{
    public static void Main()
    {
        CreateHostBuilder().Build().Run();
    }

    public static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
            webBuilder.UseUrls(@"http://localhost:5000");
        });
}
