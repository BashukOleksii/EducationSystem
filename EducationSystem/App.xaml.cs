using Dapper;
using EducationSystem.Configuration;
using EducationSystem.Data;
using System.Windows;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace EducationSystem
{
    public partial class App : Application
    {
        // НОВЕ: фабрика належить композиційному кореню застосунку.
        // Її використовують обидві сторінки аналітики, не створюючи
        // другого рядка підключення чи окремого глобального з'єднання.
        public DatabaseConnectionFactory DatabaseFactory { get; private set; } = null!;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DefaultTypeMap.MatchNamesWithUnderscores = true;

            try
            {
                AppSettings settings = AppSettings.Load();

                // НОВЕ: зберігаємо фабрику для інших модулів аналітики.
                DatabaseFactory = new DatabaseConnectionFactory(
                    settings.ConnectionStrings.DefaultConnection);

                /*
                 * Seeder запускається ДО відкриття MainWindow.
                 * Якщо база порожня -> додає тестові дані.
                 * Якщо є хоча б один запис -> нічого не робить.
                 */
                DatabaseSeeder seeder = new DatabaseSeeder(DatabaseFactory);
                await seeder.SeedIfEmptyAsync();

                MainWindow mainWindow = new MainWindow(DatabaseFactory);
                MainWindow = mainWindow;
                mainWindow.Show();
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    exception.Message,
                    "Помилка запуску",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}
