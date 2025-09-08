using FoxSky.StocksService.SharedServices;

namespace FoxSky.StocksService.Traders;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Hello, Traders!");

        EnvReader.Load();

        using var messageReceiver = MessageReceiver.MessageReceiver.CreateAsync().GetAwaiter().GetResult();

        messageReceiver.ReceiveMessageAsync().GetAwaiter().GetResult();
    }
}