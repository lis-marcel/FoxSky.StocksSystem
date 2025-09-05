using FoxSky.StocksService.SharedServices;

namespace FoxSky.StocksSystem.StocksProvider;

class Program
{
    private readonly static HttpClient _httpClient = new();

    public static void Main(string[] args)
    {
        EnvReader.Load();

        var apiKey = Environment.GetEnvironmentVariable("STOCKS_API_KEY");
        var apiUrl = Environment.GetEnvironmentVariable("STOCKS_API_URL");

        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiUrl))
            throw new ArgumentNullException("API key or URL is not set in environment variables.");

        _httpClient.BaseAddress = new Uri(apiUrl);

        WebServices.StocksProvider.GetStocks(_httpClient, apiKey).GetAwaiter().GetResult();
    }
}