using FoxSky.StocksService.Ceo.Database.Context;
using FoxSky.StocksService.CEO.MessageBroker;
using FoxSky.StocksService.CEO.Services;
using FoxSky.StocksSystem.SharedServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FoxSky.StocksService.CEO
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            ConfigVariablesReader.LoadEnv();

            try
            {
                Console.WriteLine("[CeoService] Starting...");

                // Ensure the database directory exists
                EnsureDatabaseDirectoryExists();

                var serviceProvider = ConfigureServices();

                // Apply migrations to ensure database is up to date
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<CeoDbContext>();
                    Console.WriteLine("[CeoService] Applying database migrations...");
                    dbContext.Database.Migrate();
                    Console.WriteLine("[CeoService] Database migrations applied successfully.");
                }

                using (var scope = serviceProvider.CreateScope())
                {
                    var messageQueueHandler = scope.ServiceProvider.GetRequiredService<ICeoMessageBroker>();
                    await messageQueueHandler.InitializeAsync();

                    Console.WriteLine("[CeoService] Initialized. Starting to send requests...");

                    var ceoService = scope.ServiceProvider.GetRequiredService<ICeoService>();
                    await ceoService.RequestReportGeneration();

                    Console.WriteLine("[CeoService] Report request sent.");

                    // Continue using messageQueueHandler within this scope
                    var processingResult = await messageQueueHandler.ReceiveMessageAsync();

                    if (!processingResult.Success)
                    {
                        Console.WriteLine($"[CeoService] Error: {processingResult.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CeoService] Fatal error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Register DbContext as scoped
            services.AddDbContextPool<CeoDbContext>(options =>
                options.UseSqlite(Environment.GetEnvironmentVariable("DB_HOST")));

            // Register CeoMessageBroker as singleton
            services.AddScoped<ICeoMessageBroker, CeoMessageBroker>();

            // Register CeoService as singleton (but now it creates DbContext instances as needed)
            services.AddScoped<ICeoService, CeoService>();

            return services.BuildServiceProvider(new ServiceProviderOptions
            {
                // This will help catch lifetime scope issues during development
                ValidateScopes = true,
                ValidateOnBuild = true
            });
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
                Console.WriteLine($"[CeoService] Using database at: {dbPath}");
            }

            // Ensure the directory exists
            var dbDirectory = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(dbDirectory) && !Directory.Exists(dbDirectory))
            {
                Console.WriteLine($"[CeoService] Creating database directory: {dbDirectory}");
                Directory.CreateDirectory(dbDirectory);
            }
        }
    }
}
