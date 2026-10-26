using Dapper;
using EducationSystem.Configuration;
using EducationSystem.Data;
using EducationSystem.Security;
using EducationSystem.Services;
using EducationSystem.Views;
using System;
using System.Windows;

using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace EducationSystem;

public partial class App : Application
{
    public DatabaseConnectionFactory DatabaseFactory { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        // На етапі входу та виходу вікна змінюються без завершення програми.
        ShutdownMode = ShutdownMode.OnExplicitShutdown;
        OpenLoginWindow();
    }

    private void OpenLoginWindow()
    {
        try
        {
            AppSettings settings = AppSettings.Load();
            AuthService authService = new AuthService(
                settings.ConnectionStrings.DefaultConnection);

            LoginWindow loginWindow = new LoginWindow(authService);
            bool? result = loginWindow.ShowDialog();

            if (result != true || loginWindow.Result is null)
            {
                Shutdown();
                return;
            }

            LoginResult loginResult = loginWindow.Result;
            DatabaseFactory = loginResult.DatabaseFactory;

            MainWindow window = new MainWindow(
                DatabaseFactory, loginResult.User);

            window.LogoutRequested += MainWindow_LogoutRequested;
            MainWindow = window;
            ShutdownMode = ShutdownMode.OnMainWindowClose;
            window.Show();
        }
        catch (Exception ex)
        {
            DatabaseFactory?.Dispose();
            MessageBox.Show(ex.Message, "Помилка запуску",
                MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
        }
    }

    private void MainWindow_LogoutRequested(object? sender, EventArgs e)
    {
        if (sender is not MainWindow previousWindow)
            return;

        ShutdownMode = ShutdownMode.OnExplicitShutdown;
        previousWindow.LogoutRequested -= MainWindow_LogoutRequested;
        previousWindow.Close();

        DatabaseFactory?.Dispose();
        DatabaseFactory = null!;

        Dispatcher.BeginInvoke(new Action(OpenLoginWindow));
    }

    protected override void OnExit(ExitEventArgs e)
    {
        DatabaseFactory?.Dispose();
        base.OnExit(e);
    }
}
