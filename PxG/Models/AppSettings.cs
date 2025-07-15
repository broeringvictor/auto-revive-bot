using System.Text.Json;

namespace PxG.Models
{
    /// <summary>
    /// Representa as configurações salvas da aplicação
    /// </summary>
    public class AppSettings
    {
        public string PokemonKey { get; set; } = "F1";
        public string ReviveKey { get; set; } = "F2";
        public string MedicineKey { get; set; } = "F3";
        
        // Posições para Revive
        public int RevivePositionX { get; set; } = 0;
        public int RevivePositionY { get; set; } = 0;
        
        // Posições para Medicine
        public int MedicinePositionX { get; set; } = 0;
        public int MedicinePositionY { get; set; } = 0;
        
        public string LastSelectedWindow { get; set; } = "";
        
        // Propriedades obsoletas mantidas para compatibilidade (serão removidas em versões futuras)
        public int PokemonBarPositionX { get; set; } = 0;
        public int PokemonBarPositionY { get; set; } = 0;
        public bool HasValidPosition { get; set; } = false;
    }

    /// <summary>
    /// Gerenciador de configurações da aplicação
    /// </summary>
    public static class SettingsManager
    {
        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "PxG",
            "settings.json"
        );

        /// <summary>
        /// Carrega as configurações salvas ou retorna configurações padrão
        /// </summary>
        public static AppSettings LoadSettings()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    string json = File.ReadAllText(SettingsPath);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json);
                    return settings ?? new AppSettings();
                }
            }
            catch (Exception ex)
            {
                // Log do erro, mas continua com configurações padrão
                Console.WriteLine($"Erro ao carregar configurações: {ex.Message}");
            }

            return new AppSettings();
        }

        /// <summary>
        /// Salva as configurações no arquivo
        /// </summary>
        public static bool SaveSettings(AppSettings settings)
        {
            try
            {
                // Cria o diretório se não existir
                string? directory = Path.GetDirectoryName(SettingsPath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Serializa e salva as configurações
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string json = JsonSerializer.Serialize(settings, options);
                File.WriteAllText(SettingsPath, json);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar configurações: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtém o caminho do arquivo de configurações
        /// </summary>
        public static string GetSettingsPath() => SettingsPath;
    }
}
