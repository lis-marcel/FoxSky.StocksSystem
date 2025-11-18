using FoxSky.StocksSystem.SharedServices;
using FoxSky.StocksService.Traders.MessageQueueHandler;
using System.Threading.Tasks;

namespace FoxSky.StocksService.Traders;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            Console.WriteLine("[Traders service] Starting...");
            EnvReader.Load();

            // Use the static CreateAsync method
            var messageQueueHandler = await MessageQueueHandler.TradersMessageBroker.CreateAsync();
            
            Console.WriteLine("[Traders service] Initialized. Starting to receive messages...");
            var processingResult = await messageQueueHandler.ReceiveMessageAsync();

            if (!processingResult.Success)
            {
                Console.WriteLine($"[Traders service] Error: {processingResult.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Traders service] Fatal error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}