using Dapper;
using EducationSystem.Configuration;
using EducationSystem.Data;
using System.Configuration;
using System.Data;
using System.Windows;

namespace EducationSystem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DefaultTypeMap.MatchNamesWithUnderscores = true;

            try
            {
                AppSettings settings = AppSettings.Load();

                DatabaseConnectionFactory databaseFactory = new DatabaseConnectionFactory(settings.ConnectionStrings.DefaultConnection);

                MainWindow mainWindow = new MainWindow(databaseFactory);

                MainWindow = mainWindow;

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

}
