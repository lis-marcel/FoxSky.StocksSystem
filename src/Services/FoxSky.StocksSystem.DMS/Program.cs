using FoxSky.StocksSystem.SharedServices;
using FoxSky.StocksSystem.DMS.MessageBroker;
using Microsoft.Extensions.DependencyInjection;
using FoxSky.StocksSystem.DMS.Services;
using MongoDB;

namespace FoxSky.StocksSystem.DMS
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("[DMS] Starting...");
                ConfigVariablesReader.LoadEnv();
                ConfigVariablesReader.LoadAppsettingsEnv();

                var serviceProvider = ConfigureServices();

                using (var scope = serviceProvider.CreateScope())
                {
                    var messageQueueHandler = scope.ServiceProvider.GetRequiredService<IDMSMessageBroker>();
                    await messageQueueHandler.InitializeAsync();

                    Console.WriteLine("[DMS] Initialized. Starting to receive messages...");

                    var processingResult = await messageQueueHandler.ReceiveMessageAsync();

                    if (!processingResult.Success)
                    {
                        Console.WriteLine($"[DMS] Error: {processingResult.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DMS] Fatal error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddScoped<IDMSService, DMSService>();
            services.AddScoped<IDMSMessageBroker, DMSMessageBroker>();

            return services.BuildServiceProvider(new ServiceProviderOptions
            {
                // This will help catch lifetime scope issues during development
                ValidateScopes = true,
                ValidateOnBuild = true
            });
        }
    }
}
