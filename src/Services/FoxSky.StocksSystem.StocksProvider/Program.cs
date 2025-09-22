using FoxSky.StocksService.SharedServices;
using FoxSky.StocksSystem.StocksProvider.MessageService;

namespace FoxSky.StocksSystem.StocksProvider;

class Program
{
    private readonly static HttpClient _httpClient = new();

    public static async Task Main(string[] args)
    {
        EnvReader.Load();

        var apiKey = Environment.GetEnvironmentVariable("STOCKS_API_KEY");
        var apiUrl = Environment.GetEnvironmentVariable("STOCKS_API_URL");

        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiUrl))
            throw new ArgumentNullException("API key or URL is not set in environment variables.");

        _httpClient.BaseAddress = new Uri(apiUrl);

        using var messageQueue = await StocksProviderMessageBroker.CreateAsync();

        var stocksResult = await Service.StocksProviderService.GetStocks(_httpClient, apiKey);

        if (stocksResult.Success)
        {
            await messageQueue.PublishMessageAsync(stocksResult.Data!.ToString()!);
            Console.WriteLine("[StocksService] Sent stock data to 'stocks' queue");
        }
    }
}