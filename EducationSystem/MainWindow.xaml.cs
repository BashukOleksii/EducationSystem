using EducationSystem.Data;
using EducationSystem.Repositories;
using EducationSystem.Security;
using EducationSystem.Services;
using EducationSystem.ViewModels;
using EducationSystem.Views;
using System.Windows;
using System.Windows.Controls;

namespace EducationSystem
{
    public partial class MainWindow : Window
    {
        private readonly DatabaseConnectionFactory _databaseFactory;


        private readonly UserSession _currentUser;

        public MainWindow(
            DatabaseConnectionFactory databaseFactory,
            UserSession currentUser)
        {
            InitializeComponent();

            _databaseFactory = databaseFactory;
            _currentUser = currentUser;

            ConfigureUserInterface();
        }

        private void ConfigureUserInterface()
        {
            if (_currentUser.Role == UserRole.Administration)
            {
                ShowGroups();
                return;
            }

            foreach (UIElement element in NavigationPanel.Children)
            {
                if (element is System.Windows.Controls.Button button)
                {
                    button.Visibility = Visibility.Collapsed;
                }
            }

            string roleName =
                _currentUser.Role switch
                {
                    UserRole.Teacher => "Викладач",
                    UserRole.Admin => "Адміністратор",
                    _ => "Адміністрація"
                };

            MainContent.Content = new TextBlock
            {
                Text =
                    $"Вхід виконано успішно!\n\n" +
                    $"Користувач: {_currentUser.Login}\n" +
                    $"Роль: {roleName}\n\n" +
                    "Функціонал цієї ролі буде підключено " +
                    "на наступному етапі.",

                FontSize = 18,
                Margin = new Thickness(24),
                TextWrapping = TextWrapping.Wrap
            };
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

        private void TeacherSubjectsButton_Click(
            object sender,
            RoutedEventArgs e)
                {
                    ShowTeacherSubjects();
                }


        private void ShowTeacherSubjects()
        {
            TeacherRepository teacherRepository =
                new TeacherRepository(
                    _databaseFactory
                );


            SubjectRepository subjectRepository =
                new SubjectRepository(
                    _databaseFactory
                );


            TeacherSubjectRepository teacherSubjectRepository =
                new TeacherSubjectRepository(
                    _databaseFactory
                );


            TeacherService teacherService =
                new TeacherService(
                    teacherRepository
                );


            SubjectService subjectService =
                new SubjectService(
                    subjectRepository
                );


            TeacherSubjectService teacherSubjectService =
                new TeacherSubjectService(
                    teacherSubjectRepository,
                    teacherRepository,
                    subjectRepository
                );


            TeacherSubjectsViewModel viewModel =
                new TeacherSubjectsViewModel(
                    teacherSubjectService,
                    teacherService,
                    subjectService
                );


            MainContent.Content =
                new TeacherSubjectsView(
                    viewModel
                );
        }

        private void LessonsButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            ShowLessons();
        }

        private void ShowLessons()
        {
            TeacherRepository teacherRepository =
                new TeacherRepository(
                    _databaseFactory
                );


            SubjectRepository subjectRepository =
                new SubjectRepository(
                    _databaseFactory
                );


            GroupRepository groupRepository =
                new GroupRepository(
                    _databaseFactory
                );


            TeacherSubjectRepository teacherSubjectRepository =
                new TeacherSubjectRepository(
                    _databaseFactory
                );


            LessonRepository lessonRepository =
                new LessonRepository(
                    _databaseFactory
                );


            TeacherService teacherService =
                new TeacherService(
                    teacherRepository
                );


            SubjectService subjectService =
                new SubjectService(
                    subjectRepository
                );


            GroupService groupService =
                new GroupService(
                    groupRepository
                );


            TeacherSubjectService teacherSubjectService =
                new TeacherSubjectService(
                    teacherSubjectRepository,
                    teacherRepository,
                    subjectRepository
                );


            LessonService lessonService =
                new LessonService(
                    lessonRepository,
                    teacherSubjectRepository,
                    groupRepository
                );


            LessonsViewModel viewModel =
                new LessonsViewModel(
                    lessonService,
                    teacherSubjectService,
                    teacherService,
                    subjectService,
                    groupService
                );


            MainContent.Content =
                new LessonsView(
                    viewModel
                );
        }

        private void JournalButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            ShowJournal();
        }

        private void ShowJournal()
        {
            GroupRepository groupRepository =
                new GroupRepository(
                    _databaseFactory
                );


            StudentRepository studentRepository =
                new StudentRepository(
                    _databaseFactory
                );


            TeacherSubjectRepository teacherSubjectRepository =
                new TeacherSubjectRepository(
                    _databaseFactory
                );


            LessonRepository lessonRepository =
                new LessonRepository(
                    _databaseFactory
                );


            JournalRepository journalRepository =
                new JournalRepository(
                    _databaseFactory
                );


            LessonService lessonService =
                new LessonService(
                    lessonRepository,
                    teacherSubjectRepository,
                    groupRepository
                );


            JournalService journalService =
                new JournalService(
                    journalRepository,
                    lessonRepository,
                    studentRepository
                );


            JournalViewModel viewModel =
                new JournalViewModel(
                    journalService,
                    lessonService
                );


            MainContent.Content =
                new JournalView(
                    viewModel
                );
        }

        private void AnalyticsButton_Click(
    object sender,
    RoutedEventArgs e)
        {
            ShowAnalytics();
        }

        private void ShowAnalytics()
        {

            AnalyticsRepository analyticsRepository =
                new AnalyticsRepository(
                    _databaseFactory
                );


            TeacherRepository teacherRepository =
                new TeacherRepository(
                    _databaseFactory
                );


            SubjectRepository subjectRepository =
                new SubjectRepository(
                    _databaseFactory
                );


            StudentRepository studentRepository =
                new StudentRepository(
                    _databaseFactory
                );


            GroupRepository groupRepository =
                new GroupRepository(
                    _databaseFactory
                );

            AnalyticsService analyticsService =
                new AnalyticsService(
                    analyticsRepository
                );


            TeacherService teacherService =
                new TeacherService(
                    teacherRepository
                );


            SubjectService subjectService =
                new SubjectService(
                    subjectRepository
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


            GroupReportExportService groupReportExportService = new GroupReportExportService();

            AnalyticsViewModel viewModel =
                new AnalyticsViewModel(
                    analyticsService,
                    teacherService,
                    subjectService,
                    studentService,
                    groupService,
                    groupReportExportService
                );


            MainContent.Content =
                new AnalyticsView(
                    viewModel
                );
        }
    }
}