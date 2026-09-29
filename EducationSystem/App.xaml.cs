using Dapper;
using EducationSystem.Configuration;
using EducationSystem.Data;
using System.Windows;

namespace EducationSystem
{
    public partial class App : Application
    {
        protected override async void OnStartup(
            StartupEventArgs e)
        {
            base.OnStartup(e);

            DefaultTypeMap.MatchNamesWithUnderscores = true;


            try
            {
                AppSettings settings =
                    AppSettings.Load();


                DatabaseConnectionFactory databaseFactory =
                    new DatabaseConnectionFactory(
                        settings.ConnectionStrings.DefaultConnection
                    );


                /*
                 * Seeder запускається ДО відкриття MainWindow.
                 *
                 * Якщо база порожня -> додає тестові дані.
                 * Якщо є хоча б один запис -> нічого не робить.
                 */
                DatabaseSeeder seeder =
                    new DatabaseSeeder(
                        databaseFactory
                    );


                await seeder
                    .SeedIfEmptyAsync();


                MainWindow mainWindow =
                    new MainWindow(
                        databaseFactory
                    );


                MainWindow =
                    mainWindow;


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
