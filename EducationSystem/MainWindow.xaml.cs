using EducationSystem.Data;
using EducationSystem.Repositories;
using EducationSystem.Security;
using EducationSystem.Services;
using EducationSystem.ViewModels;
using EducationSystem.Views;
using System;
using System.Windows;
using System.Windows.Controls;

namespace EducationSystem;

public partial class MainWindow : Window
{
    private readonly DatabaseConnectionFactory _databaseFactory;
    private readonly UserSession _currentUser;
    public event EventHandler? LogoutRequested;

    public MainWindow(
        DatabaseConnectionFactory databaseFactory,
        UserSession currentUser)
    {
        InitializeComponent();

        _databaseFactory = databaseFactory;
        _currentUser = currentUser;

        ConfigureUserInterface();
    }

    private bool HasRole(params UserRole[] allowedRoles)
    {
        return Array.IndexOf(allowedRoles, _currentUser.Role) >= 0;
    }

    private void ConfigureUserInterface()
    {
        bool isTeacher = HasRole(UserRole.Teacher);
        bool isAdministration = HasRole(UserRole.Administration);
        bool isAdmin = HasRole(UserRole.Admin);

        CurrentUserText.Text = _currentUser.Login;
        CurrentRoleText.Text = _currentUser.Role switch
        {
            UserRole.Teacher => "Викладач",
            UserRole.Administration => "Адміністрація",
            UserRole.Admin => "Адміністратор системи",
            _ => "Невідома роль"
        };

        GroupsButton.Visibility = isAdministration
            ? Visibility.Visible : Visibility.Collapsed;
        SubjectsButton.Visibility = isAdministration
            ? Visibility.Visible : Visibility.Collapsed;
        TeachersButton.Visibility = isAdministration
            ? Visibility.Visible : Visibility.Collapsed;
        StudentsButton.Visibility = isAdministration
            ? Visibility.Visible : Visibility.Collapsed;
        TeacherSubjectsButton.Visibility = isAdministration
            ? Visibility.Visible : Visibility.Collapsed;

        LessonsButton.Visibility = (isTeacher || isAdministration)
            ? Visibility.Visible : Visibility.Collapsed;
        JournalButton.Visibility = isTeacher
            ? Visibility.Visible : Visibility.Collapsed;
        AnalyticsButton.Visibility = isAdministration
            ? Visibility.Visible : Visibility.Collapsed;
        UsersButton.Visibility = isAdmin
            ? Visibility.Visible : Visibility.Collapsed;

        LogoutButton.Visibility = Visibility.Visible;

        if (isAdministration)
        {
            ShowGroups();
        }
        else if (isAdmin)
        {
            ShowUsersPage();
        }
        else
        {
            ShowWelcomePage();
        }
    }

    private void ShowWelcomePage()
    {
        MainContent.Content = new TextBlock
        {
            Text = $"Вітаємо, {_currentUser.Login}!\n\n" +
                   "Ваша роль: Викладач\n\n" +
                   "Доступні розділи: «Заняття» та «Журнал».",
            FontSize = 20,
            Margin = new Thickness(30),
            TextWrapping = TextWrapping.Wrap
        };
    }

    private void ShowUsersPage()
    {
        MainContent.Content = new TextBlock
        {
            Text = "Керування користувачами\n\n" +
                   "Тут будуть реалізовані:\n" +
                   "• Створення облікових записів\n" +
                   "• Призначення ролей\n" +
                   "• Блокування користувачів\n" +
                   "• Видалення облікових записів",
            FontSize = 20,
            Margin = new Thickness(30),
            TextWrapping = TextWrapping.Wrap
        };
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        LogoutButton.IsEnabled = false;
        LogoutRequested?.Invoke(this, EventArgs.Empty);
    }

    private void UsersButton_Click(object sender, RoutedEventArgs e)
    {
        if (!HasRole(UserRole.Admin)) return;
        ShowUsersPage();
    }

    private void GroupsButton_Click(object sender, RoutedEventArgs e)
    {
        if (!HasRole(UserRole.Administration)) return;
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

    private void SubjectsButton_Click(object sender, RoutedEventArgs e)
    {
        if (!HasRole(UserRole.Administration)) return;
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

    private void TeachersButton_Click(object sender, RoutedEventArgs e)
    {
        if (!HasRole(UserRole.Administration)) return;
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

    private void StudentsButton_Click(object sender, RoutedEventArgs e)
    {
        if (!HasRole(UserRole.Administration)) return;
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

    private void TeacherSubjectsButton_Click(object sender, RoutedEventArgs e)
    {
        if (!HasRole(UserRole.Administration)) return;
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

    private void LessonsButton_Click(object sender, RoutedEventArgs e)
    {
        if (!HasRole(UserRole.Teacher, UserRole.Administration)) return;
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

    private void JournalButton_Click(object sender, RoutedEventArgs e)
    {
        if (!HasRole(UserRole.Teacher)) return;
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

    private void AnalyticsButton_Click(object sender, RoutedEventArgs e)
    {
        if (!HasRole(UserRole.Administration)) return;
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
