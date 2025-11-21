using System.Text.Json;

namespace FoxSky.StocksSystem.SharedServices;

public class ConfigVariablesReader
{
    public static void LoadEnv()
    {
        string fileName = ".env";
        var filePath = FindConfigFile(fileName);

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
        string fileName = "appsettings.json";
        var filePath = FindConfigFile(fileName);

        if (!File.Exists(filePath) || filePath == null)
            throw new FileNotFoundException($"The file '{filePath}' does not exist.");

        var jsonString = File.ReadAllText(filePath);
        using var doc = JsonDocument.Parse(jsonString);
        var root = doc.RootElement;

        ProcessJsonElement(root);
    }

    private static void ProcessJsonElement(JsonElement element, string? prefix = null)
    {
        foreach (var property in element.EnumerateObject())
        {
            var currentKey = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}:{property.Name}";

            if (property.Value.ValueKind == JsonValueKind.Object)
            {
                ProcessJsonElement(property.Value, currentKey);
            }
            else
            {
                var value = property.Value.ToString();
                AppContext.SetData(currentKey, value);
            }
        }
    }

    private static string? FindConfigFile(string fileName)
    {
        DirectoryInfo currentDir = new(AppDomain.CurrentDomain.BaseDirectory);

        while (currentDir != null)
        {
            string configPath = Path.Combine(currentDir.FullName, $"Config\\{fileName}");
            if (File.Exists(configPath))
            {
                return configPath;
            }

            currentDir = currentDir.Parent!;
        }

        return null;
    }
}