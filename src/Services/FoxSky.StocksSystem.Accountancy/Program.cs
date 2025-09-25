using FoxSky.StocksService.SharedServices;
using FoxSky.StocksSystem.Accountancy.Database.Context;
using FoxSky.StocksSystem.Accountancy.MessageBroker;
using FoxSky.StocksSystem.Accountancy.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace FoxSky.StocksSystem.Accountancy;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            Console.WriteLine("[AccountancyService] Starting...");
            EnvReader.Load();

            // Ensure the database directory exists
            EnsureDatabaseDirectoryExists();
            
            var serviceProvider = ConfigureServices();

            // Apply migrations to ensure database is up to date
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AccountancyDbContext>();
                Console.WriteLine("[AccountancyService] Applying database migrations...");
                dbContext.Database.Migrate();
                Console.WriteLine("[AccountancyService] Database migrations applied successfully.");
            }

            // Resolve the message broker from the service provider
            var messageQueueHandler = serviceProvider.GetRequiredService<IAccountancyMessageBroker>();
            await messageQueueHandler.InitializeAsync();

            Console.WriteLine("[AccountancyService] Initialized. Starting to receive messages...");

            var processingResult = await messageQueueHandler.ReceiveMessageAsync();

            if (!processingResult.Success)
            {
                Console.WriteLine($"[AccountancyService] Error: {processingResult.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AccountancyService] Fatal error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }

    private static void EnsureDatabaseDirectoryExists()
    {
        var connectionString = Environment.GetEnvironmentVariable("DB_HOST") ?? string.Empty;
        
        // Extract the database path from the connection string
        var dataSourcePart = connectionString.Split(';')
            .FirstOrDefault(part => part.Trim().StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase));

        DirectoryInfo currentDir = new(AppDomain.CurrentDomain.BaseDirectory);
        string dbFolderPath = string.Empty;

        while (currentDir != null)
        {
            dbFolderPath = Path.Combine(currentDir.FullName, "Database");

            if (Directory.Exists(dbFolderPath))
            {
                currentDir = new DirectoryInfo(dbFolderPath);
                break;
            }

            currentDir = currentDir.Parent!;
        }

        var dbPath = dataSourcePart!.Substring("Data Source=".Length).Trim();

        if (dataSourcePart != null)
        {
            
            // Resolve the path relative to the current directory
            dbPath = Path.Combine(currentDir!.ToString(), dbPath);
                
            // Update the connection string with the absolute path
            var newConnectionString = connectionString.Replace(dataSourcePart, $"Data Source={dbPath}");
            Environment.SetEnvironmentVariable("DB_HOST", newConnectionString);
            Console.WriteLine($"[AccountancyService] Using database at: {dbPath}");
        }
            
        // Ensure the directory exists
        var dbDirectory = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrEmpty(dbDirectory) && !Directory.Exists(dbDirectory))
        {
            Console.WriteLine($"[AccountancyService] Creating database directory: {dbDirectory}");
            Directory.CreateDirectory(dbDirectory);
        }
    }

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Register DbContext as scoped
        services.AddDbContext<AccountancyDbContext>(options =>
            options.UseSqlite(Environment.GetEnvironmentVariable("DB_HOST")));

        // Register AccountancyService as singleton (but now it creates DbContext instances as needed)
        services.AddSingleton<IAccountancyService, AccountancyService>();
        
        // Register AccountancyMessageBroker as singleton
        services.AddSingleton<IAccountancyMessageBroker, AccountancyMessageBroker>();

        return services.BuildServiceProvider(new ServiceProviderOptions
        {
            // This will help catch lifetime scope issues during development
            ValidateScopes = true,
            ValidateOnBuild = true
        });
    }
}