using FoxSky.StocksService.SharedServices;
using FoxSky.StocksSystem.Accountancy.MessageBroker;

namespace FoxSky.StocksSystem.Accountancy;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("[Accountancy service] Starting...");
            EnvReader.Load();

            var messageQueueHandler = AccountancyMessageBroker.CreateAsync().GetAwaiter().GetResult();

            Console.WriteLine("[Accountancy service] Initialized. Starting to receive messages...");
            var processingResult = messageQueueHandler.ReceiveMessageAsync().GetAwaiter().GetResult();

            if (!processingResult.Success)
            {
                Console.WriteLine($"[Accountancy service] Error: {processingResult.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Accountancy service] Fatal error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}