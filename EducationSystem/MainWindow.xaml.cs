using System.Windows;
using EducationSystem.Data;
using EducationSystem.Repositories;
using EducationSystem.Services;
using EducationSystem.ViewModels;
using EducationSystem.Views;

namespace EducationSystem
{
    public partial class MainWindow : Window
    {
        private readonly DatabaseConnectionFactory _databaseFactory;

        public MainWindow(
            DatabaseConnectionFactory databaseFactory)
        {
            InitializeComponent();

            _databaseFactory =
                databaseFactory;

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


        private void SubjectsButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            ShowSubjects();
        }

        private void ShowSubjects()
        {
            SubjectRepository repository =
                new SubjectRepository(
                    _databaseFactory
                );

            SubjectService service =
                new SubjectService(
                    repository
                );

            SubjectsViewModel viewModel =
                new SubjectsViewModel(
                    service
                );

            MainContent.Content =
                new SubjectsView(
                    viewModel
                );
        }
    }
}