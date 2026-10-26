
using EducationSystem.Security;
using EducationSystem.Services;
using Microsoft.Data.SqlClient;
using System.Security;
using System.Windows;

namespace EducationSystem.Views;

public partial class LoginWindow : Window
{
    private readonly AuthService _authService;

    public LoginResult? Result { get; private set; }

    public LoginWindow(AuthService authService)
    {
        InitializeComponent();

        _authService = authService;
    }

    private async void LoginButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        LoginButton.IsEnabled = false;
        ErrorText.Text = string.Empty;

        try
        {
            string login = LoginTextBox.Text;

            using SecureString password =
                PasswordInput.SecurePassword;

            Result = await _authService.LoginAsync(
                login,
                password
            );

            PasswordInput.Clear();

            DialogResult = true;
        }
        catch (SqlException ex)
            when (ex.Number == 18456)
        {
            ErrorText.Text =
                "Неправильний логін або пароль, " +
                "або вхід для цього користувача заборонено.";
        }
        catch (SqlException)
        {
            ErrorText.Text =
                "Помилка підключення до SQL Server. " +
                "Перевірте доступність сервера та бази даних.";
        }
        catch (UnauthorizedAccessException ex)
        {
            ErrorText.Text = ex.Message;
        }
        catch (ArgumentException ex)
        {
            ErrorText.Text = ex.Message;
        }
        catch (Exception)
        {
            ErrorText.Text =
                "Не вдалося виконати авторизацію.";
        }
        finally
        {
            LoginButton.IsEnabled = true;
        }
    }
}
