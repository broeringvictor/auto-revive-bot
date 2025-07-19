using System.Text.Json;
using PxG.Models;

namespace PxG
{
    public static class SettingsManager
    {
        private static readonly string SettingsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
            "PxG", 
            "settings.json"
        );

        public static AppSettings LoadSettings()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    var json = File.ReadAllText(SettingsFilePath);
                    return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
            }
            catch (Exception ex)
            {
                // Log error if needed
                Console.WriteLine($"Erro ao carregar configurações: {ex.Message}");
            }

            return new AppSettings();
        }

        public static bool SaveSettings(AppSettings settings)
        {
            try
            {
                var directory = Path.GetDirectoryName(SettingsFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory!);
                }

                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(SettingsFilePath, json);
                return true;
            }
            catch (Exception ex)
            {
                // Log error if needed
                Console.WriteLine($"Erro ao salvar configurações: {ex.Message}");
                return false;
            }
        }
    }
}
