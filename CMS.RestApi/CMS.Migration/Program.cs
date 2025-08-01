// See https://aka.ms/new-console-template for more information
using CMS.Core.Common;
using CMS.Infrastructure.Common;
using FluentMigrator.Builders.Create.Index;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Logging;
using LinqToDB.Common;
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
    var dir = Directory.GetCurrentDirectory();
    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(dir)
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build();
    AppData.Configuration = config; // Store the configuration in a static class for global access
    // Assumes your connection string section is named "ConnectionStrings" and has a property "CMSConnString"
    var connectionString = config.GetSection("ConnectionStrings")["CMSConnString"];

    return new ServiceCollection()
        .AddFluentMigratorCore()
        .ConfigureRunner(rb =>
        {
            rb = rb.AddMySql5()
                .WithGlobalConnectionString(connectionString)
                .WithGlobalCommandTimeout(TimeSpan.FromMinutes(530))
                .ScanIn(typeof(Program).Assembly).For.Migrations();
        })
        .AddLogging(lb =>
        {
            lb.AddFluentMigratorConsole();
        })
        .AddSingleton<ILoggerProvider, LogFileFluentMigratorLoggerProvider>()
        .Configure<FluentMigratorLoggerOptions>(options =>
        {
            options.ShowSql = true;
            options.ShowElapsedTime = true;
        })
        .BuildServiceProvider(false);
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