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

        private void TeachersButton_Click(
           object sender,
           RoutedEventArgs e)
        {
            ShowTeachers();
        }

        private void ShowTeachers()
        {
            TeacherRepository repository =
                new TeacherRepository(
                    _databaseFactory
                );

            TeacherService service =
                new TeacherService(
                    repository
                );

            TeachersViewModel viewModel =
                new TeachersViewModel(
                    service
                );

            MainContent.Content =
                new TeachersView(
                    viewModel
                );
        }

        private void StudentsButton_Click(
    object sender,
    RoutedEventArgs e)
        {
            ShowStudents();
        }


        private void ShowStudents()
        {
            GroupRepository groupRepository =
                new GroupRepository(
                    _databaseFactory
                );


            StudentRepository studentRepository =
                new StudentRepository(
                    _databaseFactory
                );


            GroupService groupService =
                new GroupService(
                    groupRepository
                );


            StudentService studentService =
                new StudentService(
                    studentRepository,
                    groupRepository
                );


            StudentsViewModel viewModel =
                new StudentsViewModel(
                    studentService,
                    groupService
                );


            MainContent.Content =
                new StudentsView(
                    viewModel
                );
        }
    }
}