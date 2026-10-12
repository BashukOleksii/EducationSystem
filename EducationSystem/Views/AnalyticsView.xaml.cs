using EducationSystem.ViewModels;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EducationSystem.Views;

public partial class AnalyticsView : UserControl
{
    private readonly AnalyticsViewModel _viewModel;

    public AnalyticsView(AnalyticsViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;

        AddGroupEnrollmentTab();

        Loaded += AnalyticsView_Loaded;
    }

    private void AddGroupEnrollmentTab()
    {
        if (Content is not Grid root)
        {
            throw new InvalidOperationException(
                "Не знайдено кореневий Grid на сторінці аналітики.");
        }

        TabControl? mainTabs = root.Children
            .OfType<TabControl>()
            .FirstOrDefault();

        if (mainTabs is null)
        {
            throw new InvalidOperationException(
                "Не знайдено основний TabControl у AnalyticsView.xaml.");
        }

        mainTabs.Items.Add(new TabItem
        {
            Header = "Контингент студентів",
            Content = new GroupEnrollmentView(_viewModel)
        });
    }

    private async void AnalyticsView_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= AnalyticsView_Loaded;
        await _viewModel.LoadAsync();
    }
}
