using FoxSky.StocksService.SharedServices;

namespace FoxSky.StocksSystem.DMS
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("[DMS] Starting...");
            EnvReader.Load();
        }
    }
}
