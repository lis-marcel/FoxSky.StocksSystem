using FoxSky.StocksService.SharedServices;

namespace FoxSky.StocksSystem.Accountancy;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("[Traders service]");
            EnvReader.Load();

            var messageQueueHandler = new MessageQueueHandler.MessageQueueHandler().CreateAsync().GetAwaiter().GetResult();

            var processingResult = messageQueueHandler.ReceiveMessageAsync().GetAwaiter().GetResult();

            if (!processingResult.Success)
            {
                Console.WriteLine($"[Traders service] Error: {processingResult.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Traders service] Fatal error: {ex.Message}");
            return;
        }
    }
}