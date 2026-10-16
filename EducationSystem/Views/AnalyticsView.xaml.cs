using EducationSystem.Data;
using EducationSystem.Repositories;
using EducationSystem.Services;
using EducationSystem.ViewModels;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Application = System.Windows.Application;

namespace EducationSystem.Views;

public partial class AnalyticsView : System.Windows.Controls.UserControl
{
    private readonly AnalyticsViewModel _viewModel;

    public AnalyticsView(AnalyticsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
        AddExtraTabs();
        Loaded += AnalyticsView_Loaded;
    }

    private void AddExtraTabs()
    {
        if (Content is not Grid root)
            throw new InvalidOperationException("Не знайдено кореневий Grid сторінки аналітики.");

        System.Windows.Controls.TabControl? mainTabs = root.Children.OfType<System.Windows.Controls.TabControl>().FirstOrDefault();
        if (mainTabs is null)
            throw new InvalidOperationException("Не знайдено TabControl сторінки аналітики.");

        mainTabs.Items.Add(new TabItem
        {
            Header = "Контингент студентів",
            Content = new GroupEnrollmentView(_viewModel)
        });

        DatabaseConnectionFactory factory = ((App)Application.Current).DatabaseFactory;
        GroupService groupService = new(new GroupRepository(factory));
        PerformanceService performanceService = new(new PerformanceRepository(factory));
        PerformanceReportExportService exporter = new();
        PerformanceViewModel performanceViewModel = new(
            performanceService, groupService, exporter);

        mainTabs.Items.Add(new TabItem
        {
            Header = "Успішність",
            Content = new PerformanceView(performanceViewModel)
        });

        AttendanceAnalyticsService attendanceService = new(
            new AttendanceAnalyticsRepository(factory));
        SubjectService subjectService = new(new SubjectRepository(factory));
        AttendanceReportExportService attendanceExporter = new();
        AttendanceViewModel attendanceViewModel = new(
            attendanceService,
            groupService,
            subjectService,
            attendanceExporter);

        mainTabs.Items.Add(new TabItem
        {
            Header = "Відвідуваність",
            Content = new AttendanceView(attendanceViewModel)
        });


        // Репозиторій статистики занять
        LearningDynamicsRepository dynamicsRepository =
            new LearningDynamicsRepository(factory);

        // Сервіс
        LearningDynamicsService dynamicsService =
            new LearningDynamicsService(dynamicsRepository);

        // ViewModel
        LearningDynamicsViewModel dynamicsViewModel =
            new LearningDynamicsViewModel(dynamicsService);

        // Нова вкладка
        mainTabs.Items.Add(new TabItem
        {
            Header = "Динаміка занять (RDLC)",
            Content = new LearningDynamicsView(dynamicsViewModel)
        });

    }

    private async void AnalyticsView_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= AnalyticsView_Loaded;
        await _viewModel.LoadAsync();
    }
}
