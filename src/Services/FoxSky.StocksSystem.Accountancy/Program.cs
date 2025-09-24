using FoxSky.StocksService.SharedServices;
using FoxSky.StocksSystem.Accountancy.Database.Context;
using FoxSky.StocksSystem.Accountancy.MessageBroker;
using FoxSky.StocksSystem.Accountancy.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FoxSky.StocksSystem.Accountancy;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            Console.WriteLine("[AccountancyService] Starting...");
            EnvReader.Load();

            var serviceProvider = ConfigureServices();

            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AccountancyDbContext>();
            
            // Apply migrations to ensure database is up to date
            Console.WriteLine("[AccountancyService] Applying database migrations...");
            dbContext.Database.Migrate();
            Console.WriteLine("[AccountancyService] Database migrations applied successfully.");

            var accountancyService = scope.ServiceProvider.GetRequiredService<IAccountancyService>();

            var messageQueueHandler = new AccountancyMessageBroker(dbContext, accountancyService);
            await messageQueueHandler.InitializeAsync();

            Console.WriteLine("[AccountancyService] Initialized. Starting to receive messages...");

            var processingResult = messageQueueHandler.ReceiveMessageAsync().GetAwaiter().GetResult();

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

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        var db = Environment.GetEnvironmentVariable("DB_HOST");

        services.AddDbContext<AccountancyDbContext>(options =>
            options.UseSqlite(Environment.GetEnvironmentVariable("DB_HOST")));

        services.AddSingleton<IAccountancyMessageBroker, AccountancyMessageBroker>();
        services.AddScoped<IAccountancyService, AccountancyService>();

        return services.BuildServiceProvider();
    }
}