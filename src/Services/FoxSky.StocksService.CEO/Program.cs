using FoxSky.StocksService.CEO.MessageBroker;
using FoxSky.StocksService.CEO.Services;
using FoxSky.StocksSystem.SharedServices;
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
                Console.WriteLine("[CEOService] Starting...");

                var serviceProvider = ConfigureServices();

                // Resolve the message broker from the service provider

                using (var scope = serviceProvider.CreateScope())
                {
                    var messageQueueHandler = scope.ServiceProvider.GetRequiredService<ICEOMessageBroker>();
                    await messageQueueHandler.InitializeAsync();

                    Console.WriteLine("[CEOService] Initialized. Starting to send requests...");

                    string datesRange = "2025-01-05 09:30:00,2025-01-12 15:05:00"; // Example date range

                    await messageQueueHandler.PublishMessageAsync(datesRange);
                    Console.WriteLine("[CEOService] Request sent. Press any key to exit...");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CEOService] Fatal error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Register AccountancyMessageBroker as singleton
            services.AddScoped<ICEOMessageBroker, CEOMessageBroker>();

            // Register AccountancyService as singleton (but now it creates DbContext instances as needed)
            services.AddScoped<ICEOService, CEOService>();

            return services.BuildServiceProvider(new ServiceProviderOptions
            {
                // This will help catch lifetime scope issues during development
                ValidateScopes = true,
                ValidateOnBuild = true
            });
        }
    }
}
