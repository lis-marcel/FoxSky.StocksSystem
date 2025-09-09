using FoxSky.StocksService.SharedServices;

namespace FoxSky.StocksSystem.StocksProvider.Service;

public class StocksProviderService
{
    private readonly static string _ticker = "LMT";

    public static async Task<OperationResult> GetStocks(HttpClient httpClient, string apiKey)
    {
        try
        {
            var responseMessage = await httpClient
                .GetAsync($"/v2/aggs/ticker/{_ticker}/prev?adjusted=true&apiKey={apiKey}");

            if (!responseMessage.IsSuccessStatusCode)
            {
                Console.WriteLine("Failed to download data.");
            }

            var strippedResponse = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            return OperationResult.Succeeded(data: strippedResponse);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting to the stock service: {ex.Message}");
            return OperationResult.Failed(message: ex.Message);
        }
    }
}