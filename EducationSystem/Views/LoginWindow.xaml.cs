using EducationSystem.Security;
using EducationSystem.Services;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
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
        LoginTextBox.Focus();
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        LoginButton.IsEnabled = false;
        ErrorText.Text = string.Empty;
        ErrorText.Visibility = Visibility.Collapsed;

        try
        {
            using SecureString password = PasswordInput.SecurePassword;

            Result = await _authService.LoginAsync(
                LoginTextBox.Text,
                password);

            PasswordInput.Clear();
            DialogResult = true;
        }
        catch (SqlException ex) when (ex.Number == 18456)
        {
            ShowError("Неправильний логін або пароль або вхід заборонений.");
        }
        catch (SqlException ex) when (ex.Number == 51050 || ex.Number == 51051)
        {
            ShowError(ex.Message);
        }
        catch (SqlException ex)
        {
            Debug.WriteLine(ex.ToString());
#if DEBUG
            ShowError($"SQL Server ({ex.Number}): {ex.Message}");
#else
            ShowError("Не вдалося підключитися до SQL Server.");
#endif
        }
        catch (UnauthorizedAccessException ex)
        {
            ShowError(ex.Message);
        }
        catch (ArgumentException ex)
        {
            ShowError(ex.Message);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
#if DEBUG
            ShowError(ex.Message);
#else
            ShowError("Сталася помилка під час авторизації.");
#endif
        }
        finally
        {
            LoginButton.IsEnabled = true;
        }
    }

    private void ShowError(string message)
    {
        ErrorText.Text = message;
        ErrorText.Visibility = Visibility.Visible;
    }
}
