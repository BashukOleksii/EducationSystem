using EducationSystem.DTOs.Security;
using EducationSystem.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;

namespace EducationSystem.Views;

public partial class AdminUsersView : System.Windows.Controls.UserControl
{
    private readonly ManagedAppUserService _service;
    private readonly ObservableCollection<ManagedAppUser> _users = new();
    private bool _loading;

    public AdminUsersView(ManagedAppUserService service)
    {
        _service = service;
        InitializeComponent();
        UsersGrid.ItemsSource = _users;
        Loaded += AdminUsersView_Loaded;
        TeacherPicker.IsEnabled = true;
    }

    private async void AdminUsersView_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= AdminUsersView_Loaded;
        await RefreshAsync();
    }

    private string SelectedRole => RolePicker.SelectedIndex == 0 ? "teacher" : "administration";

    private int? SelectedTeacher => SelectedRole == "teacher"
        ? TeacherPicker.SelectedValue is int id ? id : null
        : null;

    private ManagedAppUser? SelectedUser => UsersGrid.SelectedItem as ManagedAppUser;

    private void RolePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (TeacherPicker is not null)
            TeacherPicker.IsEnabled = SelectedRole == "teacher";
    }

    private void UsersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SelectedInfo is null) return;
        if (SelectedUser is not ManagedAppUser user)
        {
            SelectedInfo.Text = "Оберіть запис у таблиці для редагування.";
            return;
        }
        LoginInput.Text = user.LoginName;
        RolePicker.SelectedIndex = user.AccountRole == "teacher" ? 0 : 1;
        TeacherPicker.SelectedValue = user.TeacherId;
        SelectedInfo.Text = $"Обрано: {user.LoginName} — {user.RoleName}, {user.StatusName}";
        ToggleActiveButton.Content = user.IsActive ? "Заблокувати акаунт" : "Розблокувати акаунт";
    }

    private async Task RefreshAsync(string? selection = null)
    {
        await ExecuteUiAsync(async () =>
        {
            IReadOnlyList<ManagedTeacherOption> teachers = await _service.GetTeachersAsync();
            IReadOnlyList<ManagedAppUser> users = await _service.GetUsersAsync();
            TeacherPicker.ItemsSource = teachers;
            _users.Clear();
            foreach (ManagedAppUser user in users) _users.Add(user);
            if (selection is not null)
            {
                foreach (ManagedAppUser user in _users)
                {
                    if (user.LoginName == selection)
                    {
                        UsersGrid.SelectedItem = user;
                        break;
                    }
                }
            }
            StatusText.Text = $"Завантажено облікових записів: {_users.Count}.";
        });
    }

    private async Task ExecuteUiAsync(Func<Task> action)
    {
        if (_loading) return;
        _loading = true;
        EditPanel.IsEnabled = false;
        RefreshButton.IsEnabled = false;
        StatusText.Foreground = Brushes.DarkSlateGray;
        try
        {
            await action();
        }
        catch (Exception exception)
        {
            StatusText.Text = exception.Message;
            StatusText.Foreground = Brushes.Firebrick;
        }
        finally
        {
            RefreshButton.IsEnabled = true;
            EditPanel.IsEnabled = true;
            _loading = false;
        }
    }

    private void NewButton_Click(object sender, RoutedEventArgs e)
    {
        UsersGrid.SelectedItem = null;
        LoginInput.Text = "es_";
        PasswordInput.Clear();
        RolePicker.SelectedIndex = 0;
        TeacherPicker.SelectedIndex = -1;
        SelectedInfo.Text = "Оберіть запис у таблиці для редагування.";
        StatusText.Text = "Введіть дані нового облікового запису.";
    }

    private async void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        await RefreshAsync(SelectedUser?.LoginName);
    }

    private async void CreateButton_Click(object sender, RoutedEventArgs e)
    {
        string login = LoginInput.Text.Trim();
        string password = PasswordInput.Password;
        string role = SelectedRole;
        int? teacher = SelectedTeacher;
        await ExecuteUiAsync(async () =>
        {
            await _service.CreateAsync(login, password, role, teacher);
            StatusText.Text = $"Створено {login}. Новий користувач може авторизуватися.";
            await LoadAfterChangeAsync(login);
        });
        PasswordInput.Clear();
    }

    private async void ChangeAssignmentButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedUser is not ManagedAppUser user)
        {
            ShowSelectionError();
            return;
        }
        string role = SelectedRole;
        int? teacher = SelectedTeacher;
        await ExecuteUiAsync(async () =>
        {
            await _service.ChangeAssignmentAsync(user.LoginName, role, teacher);
            await LoadAfterChangeAsync(user.LoginName);
            StatusText.Text = "Призначення оновлено. Користувачу потрібно увійти повторно.";
        });
    }

    private async void ToggleActiveButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedUser is not ManagedAppUser user)
        {
            ShowSelectionError();
            return;
        }
        bool target = !user.IsActive;
        await ExecuteUiAsync(async () =>
        {
            await _service.SetActiveAsync(user.LoginName, target);
            await LoadAfterChangeAsync(user.LoginName);
            StatusText.Text = target
                ? "SQL Login активовано."
                : "SQL Login заблоковано для нових підключень. Уже відкриті SQL-сесії можуть ще працювати.";
        });
    }

    private async void ResetPasswordButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedUser is not ManagedAppUser user)
        {
            ShowSelectionError();
            return;
        }
        string password = PasswordInput.Password;
        await ExecuteUiAsync(async () =>
        {
            await _service.ResetPasswordAsync(user.LoginName, password);
            StatusText.Text = $"Пароль {user.LoginName} оновлено.";
        });
        PasswordInput.Clear();
    }

    private async void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedUser is not ManagedAppUser user)
        {
            ShowSelectionError();
            return;
        }
        System.Windows.MessageBoxResult answer = System.Windows.MessageBox.Show(
            $"Назавжди видалити SQL Login і Database User '{user.LoginName}'?\n\n" +
            "Навчальні дані та викладач не видаляються.",
            "Підтвердження видалення", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning);
        if (answer != System.Windows.MessageBoxResult.Yes) return;
        await ExecuteUiAsync(async () =>
        {
            await _service.DeleteAsync(user.LoginName);
            await LoadAfterChangeAsync();
            LoginInput.Text = "es_";
            PasswordInput.Clear();
            StatusText.Text = $"Акаунт {user.LoginName} видалено.";
        });
    }

    private async Task LoadAfterChangeAsync(string? selection = null)
    {
        IReadOnlyList<ManagedTeacherOption> teachers = await _service.GetTeachersAsync();
        IReadOnlyList<ManagedAppUser> users = await _service.GetUsersAsync();
        TeacherPicker.ItemsSource = teachers;
        _users.Clear();
        foreach (ManagedAppUser user in users) _users.Add(user);
        UsersGrid.SelectedItem = null;
        if (selection is null) return;
        foreach (ManagedAppUser user in _users)
        {
            if (user.LoginName == selection)
            {
                UsersGrid.SelectedItem = user;
                break;
            }
        }
    }

    private void ShowSelectionError()
    {
        StatusText.Foreground = Brushes.Firebrick;
        StatusText.Text = "Спочатку обери користувача у таблиці.";
    }
}
