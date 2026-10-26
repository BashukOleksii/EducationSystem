
using Dapper;
using EducationSystem.Configuration;
using EducationSystem.Data;
using EducationSystem.Security;
using EducationSystem.Services;
using EducationSystem.Views;
using System.Windows;

using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace EducationSystem;

public partial class App : Application
{
    public DatabaseConnectionFactory DatabaseFactory
    { get; private set; } = null!;

    protected override void OnStartup(
        StartupEventArgs e)
    {
        base.OnStartup(e);

        DefaultTypeMap.MatchNamesWithUnderscores = true;

        ShutdownMode =
            ShutdownMode.OnExplicitShutdown;

        try
        {
            AppSettings settings = AppSettings.Load();

            AuthService authService = new AuthService(
                settings.ConnectionStrings.DefaultConnection
            );

            LoginWindow loginWindow =
                new LoginWindow(authService);

            bool? loginResult = loginWindow.ShowDialog();

            if (loginResult != true ||
                loginWindow.Result is null)
            {
                Shutdown();
                return;
            }

            LoginResult result = loginWindow.Result;

            DatabaseFactory = result.DatabaseFactory;

            MainWindow mainWindow = new MainWindow(
                DatabaseFactory,
                result.User
            );

            MainWindow = mainWindow;

            ShutdownMode =
                ShutdownMode.OnMainWindowClose;

            mainWindow.Show();
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                exception.Message,
                "Помилка запуску",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );

            Shutdown();
        }
    }
}
