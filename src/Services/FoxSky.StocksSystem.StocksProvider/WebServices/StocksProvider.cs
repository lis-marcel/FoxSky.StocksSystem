namespace FoxSky.StocksSystem.StocksProvider.WebServices;

public class StocksProvider
{
    private readonly static string _ticker = "LMT";

    public static async Task GetStocks(HttpClient httpClient, string apiKey)
    {
        try
        {
            var responseMessage = await httpClient
                .GetAsync($"/v2/aggs/ticker/{_ticker}/prev?adjusted=true&apiKey={apiKey}");

            if (!responseMessage.IsSuccessStatusCode)
            {
                Console.WriteLine("Failed to download data.");
            }

            Console.WriteLine("Data downloaded successfully.");
            Console.WriteLine(responseMessage.Content.ReadAsStringAsync().Result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting to the stock service: {ex.Message}");
        }
    }
}