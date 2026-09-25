using System.IO;
using System.Text.Json;

namespace EducationSystem.Configuration;

public sealed class AppSettings
{
    public ConnectionStringSettings ConnectionStrings { get; set; } = new();

    public static AppSettings Load()
    {
        string path = Path.Combine(
            AppContext.BaseDirectory,
            "appsettings.json"
        );

        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                "Не знайдено файл appsettings.json.",
                path
            );
        }

        string json = File.ReadAllText(path);

        AppSettings? settings =
            JsonSerializer.Deserialize<AppSettings>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );

        if (settings is null)
        {
            throw new InvalidOperationException(
                "Не вдалося прочитати appsettings.json."
            );
        }

        if (string.IsNullOrWhiteSpace(
                settings.ConnectionStrings.DefaultConnection))
        {
            throw new InvalidOperationException(
                "Connection string DefaultConnection не заданий."
            );
        }

        return settings;
    }
}

public sealed class ConnectionStringSettings
{
    public string DefaultConnection { get; set; } = string.Empty;
}