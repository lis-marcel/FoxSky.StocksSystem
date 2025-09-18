using FoxSky.StocksService.SharedServices;
using FoxSky.StocksService.Traders.MessageQueueHandler;
using System.Threading.Tasks;

namespace FoxSky.StocksService.Traders;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            Console.WriteLine("[Traders service]");
            EnvReader.Load();

            var messageQueueHandler = new MessageQueueHandler.MessageQueueHandler().CreateAsync().GetAwaiter().GetResult();

            var processingResult = messageQueueHandler.ReceiveMessageAsync().GetAwaiter().GetResult();

            if (processingResult.Success)
            {
                await messageQueueHandler.PublishMessageAsync(processingResult.Data!);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Traders service] Fatal error: {ex.Message}");
            return;
        }
    }
}