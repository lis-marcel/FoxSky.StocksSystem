namespace FoxSky.StocksSystem.SharedServices;

public class ConfigVariablesReader
{
    public static void LoadEnv()
    {
        var filePath = FindEnvFile();

        if (!File.Exists(filePath) || filePath == null)
            throw new FileNotFoundException($"The file '{filePath}' does not exist.");

        foreach (var line in File.ReadAllLines(filePath))
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue; // Skip empty lines and comments

            var parts = line.Split('=', 2);

            if (parts.Length != 2)
                continue; // Skip lines that are not key-value pairs

            var key = parts[0].Trim();
            var value = parts[1].Trim();

            Environment.SetEnvironmentVariable(key, value);
        }
    }

    public static void LoadAppsettingsEnv()
    {
        var filePath = FindAppsettingsFile();

        if (!File.Exists(filePath) || filePath == null)
            throw new FileNotFoundException($"The file '{filePath}' does not exist.");

        foreach (var line in File.ReadAllLines(filePath))
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue; // Skip empty lines and comments

            var parts = line.Split('=', 2);

            if (parts.Length != 2)
                continue; // Skip lines that are not key-value pairs

            var key = parts[0].Trim();
            var value = parts[1].Trim();

            AppContext.SetData(key, value);
        }
    }

    private static string? FindEnvFile()
    {
        DirectoryInfo currentDir = new(AppDomain.CurrentDomain.BaseDirectory);

        while (currentDir != null)
        {
            string configPath = Path.Combine(currentDir.FullName, "Config\\.env");
            if (File.Exists(configPath))
            {
                return configPath;
            }

            currentDir = currentDir.Parent!;
        }

        return null;
    }

    private static string? FindAppsettingsFile()
    {
        DirectoryInfo currentDir = new(AppDomain.CurrentDomain.BaseDirectory);

        while (currentDir != null)
        {
            string configPath = Path.Combine(currentDir.FullName, "Config\\appsettings.json");
            if (File.Exists(configPath))
            {
                return configPath;
            }

            currentDir = currentDir.Parent!;
        }

        return null;
    }
}
