
using ExampleService.Services;
using Serilog;
using System.Reflection;

namespace ExampleService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigureSerilog();
            var builder = WebApplication.CreateBuilder(args);
            //builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHealthChecks();
            builder.Services.AddScoped<IAzureStorageQueueService, AzureStorageQueueService>();
            builder.Services.AddHostedService<MessageProcessor>();

            var app = builder.Build();
            app.UseHealthChecks("/_health");
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }

        private static void ConfigureSerilog()
        {
            //  Get current directory path
            var workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new Exception("ConfigureSerilog failed!, Working directory is null!");
            // Build Serilog configurations 
            var configuration = new ConfigurationBuilder()
                .SetBasePath(workingDirectory)
                .AddJsonFile("serilog.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables().Build();

            //  Set base path environment variable
            Environment.SetEnvironmentVariable("BASEDIR", workingDirectory);

            // Configure Serilog using the provided configuration
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
    }
}
