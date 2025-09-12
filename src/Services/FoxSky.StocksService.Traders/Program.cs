using FoxSky.StocksService.SharedServices;

namespace FoxSky.StocksService.Traders;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("[Traders service]");

        EnvReader.Load();

        using var messageReceiver = MessageQueueHandler.MessageQueueHandler.CreateAsync().GetAwaiter().GetResult();

        messageReceiver.ReceiveMessageAsync().GetAwaiter().GetResult();
    }
}