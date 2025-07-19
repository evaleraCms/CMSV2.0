// See https://aka.ms/new-console-template for more information
using CMS.Infrastructure.Common;
using FluentMigrator.Builders.Create.Index;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

Console.WriteLine("Hello, World!");

var serviceProvider = CreateServices();
long revertToVersion = 0; // Set to the version you want to revert to, or 0 for no revert
if (args != null)
{ 
    foreach (var arg in args)
    {
        string[] parts = arg.Split(new[] {':'},StringSplitOptions.RemoveEmptyEntries);
        var argName = parts[0].ToLower();
        if (parts.Length > 1)
        {
            switch(argName)
            {
                case "--revert":
                long.TryParse(parts[1], out revertToVersion);
                    break;
            }

        }

    }

}

UpdateDatabase(serviceProvider, revertToVersion);

static IServiceProvider CreateServices()

{
    var test = AppDomain.CurrentDomain.BaseDirectory;
    string logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", $"Migration_{DateTime.Now:yyyy-dd-MM_hh-mm-ss}.log");
    var loDir = Path.GetDirectoryName(logFile);
    var enviroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"appsettings.{enviroment}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build();
    AppData.Configuration = config;
   
    var connectionString = ConfigSettings.CMSConnStr;
    //return null;
    return new ServiceCollection()
        .AddFluentMigratorCore()
        .ConfigureRunner(rb =>
        {
            rb = rb.AddMySql5() // or .AddSQLite() for SQLite
                .WithGlobalConnectionString(connectionString)
                .WithGlobalCommandTimeout(TimeSpan.FromMinutes(530)) // Set a global command timeout if needed
                .ScanIn(typeof(Program).Assembly).For.Migrations();

        }).AddLogging(lb =>
        {
            lb.AddFluentMigratorConsole();
            
        }).AddSingleton<ILoggerProvider,LogFileFluentMigratorLoggerProvider>()
        .Configure<FluentMigratorLoggerOptions>(options =>
        {
            
            options.ShowSql = true; // Set to true to log SQL statements
            options.ShowElapsedTime = true; // Set to true to log elapsed time for each migration
        }).BuildServiceProvider(false);

}

static void UpdateDatabase(IServiceProvider serviceProvider, long revertToVersion = 0)
{
    using (var scope = serviceProvider.CreateScope())
    {
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        if (revertToVersion > 0)
        { 
            runner.MigrateDown(revertToVersion); // Revert to a specific version
        }
        else
        {
            runner.MigrateUp(); // Migrate to the latest version
        }        
    }
}