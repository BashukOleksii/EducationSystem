using System.Windows;
using EducationSystem.Data;
using EducationSystem.Repositories;
using EducationSystem.Services;
using EducationSystem.ViewModels;
using EducationSystem.Views;

namespace EducationSystem;

public partial class MainWindow : Window
{
    private readonly DatabaseConnectionFactory _databaseFactory;

    public MainWindow(
        DatabaseConnectionFactory databaseFactory)
    {
        InitializeComponent();

        _databaseFactory = databaseFactory;

        ShowGroups();
    }

    private void GroupsButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        ShowGroups();
    }

    private void ShowGroups()
    {
        GroupRepository repository =
            new GroupRepository(
                _databaseFactory
            );

        GroupService service =
            new GroupService(
                repository
            );

        GroupsViewModel viewModel =
            new GroupsViewModel(
                service
            );

        MainContent.Content =
            new GroupsView(
                viewModel
            );
    }
}