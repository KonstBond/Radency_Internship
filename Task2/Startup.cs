using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Task2.Database;

namespace Task2
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddLogging();
            services.AddSwaggerGen();
            services.AddEndpointsApiExplorer();
            services.AddDbContext<LibraryDbContext>(o =>
            {
                o.UseSqlServer(WebApplication
                            .CreateBuilder()
                            .Configuration.GetConnectionString("ConnectToDb"));
            });

            services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = HttpLoggingFields.RequestHeaders;
                logging.LoggingFields = HttpLoggingFields.RequestMethod;
                logging.LoggingFields = HttpLoggingFields.RequestBody;
                logging.LoggingFields = HttpLoggingFields.RequestQuery;
            });
            
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {          
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpLogging();
            app.UseRouting();

            app.UseEndpoints(e =>
            {
                e.MapControllers();
            });
        }
    }
}
