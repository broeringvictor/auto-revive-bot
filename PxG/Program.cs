using System.Globalization;
using PxG.Views;

namespace PxG;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        // If you want to change the application configuration, you can uncomment the line below.
        // Otherwise, the default configuration will be used.
        
        ApplicationConfiguration.Initialize();
        
        // Load application settings
        SettingsManager.LoadSettings();
        
        // Set the application culture (optional, uncomment if needed)
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");
        
        // Enable visual styles for the application
        Application.EnableVisualStyles();
        

        Application.SetCompatibleTextRenderingDefault(false);
        
        // Initialize application configuration
        ApplicationConfiguration.Initialize();
        
        /*
#pragma warning disable WFO5001
        Application.SetColorMode(SystemColorMode.Dark);
#pragma warning restore WFO5001
        */
        Application.Run(new ReviveView());
    }
}