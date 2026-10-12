using EducationSystem.ViewModels;
using EducationSystem.Visualizations;
using Microsoft.Win32;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace EducationSystem.Views;

public partial class AttendanceView : UserControl
{
    private readonly AttendanceViewModel _viewModel;
    private bool _loadedOnce;

    public AttendanceView(AttendanceViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
        Loaded += AttendanceView_Loaded;
        Unloaded += AttendanceView_Unloaded;
    }

    private async void AttendanceView_Loaded(object sender, RoutedEventArgs e)
    {
        _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        if (!_loadedOnce)
        {
            _loadedOnce = true;
            await _viewModel.LoadAsync();
        }
        DrawCharts();
    }

    private void AttendanceView_Unloaded(object sender, RoutedEventArgs e)
    {
        _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AttendanceViewModel.Report))
            DrawCharts();
    }

    private void DrawCharts()
    {
        AttendanceCharts.PopulateGroups(GroupsPlot.Plot, _viewModel.Report);
        AttendanceCharts.PopulateMonthlyCoverage(MonthsPlot.Plot, _viewModel.Report);
        GroupsPlot.Refresh();
        MonthsPlot.Refresh();
    }

    private void ExportExcel_Click(object sender, RoutedEventArgs e)
    {
        if (!CanExport()) return;
        var dialog = new SaveFileDialog
        {
            Title = "Зберегти звіт про відвідуваність (Excel)",
            Filter = "Книга Excel (*.xlsx)|*.xlsx",
            DefaultExt = ".xlsx",
            AddExtension = true,
            OverwritePrompt = true,
            FileName = $"Відвідуваність_{DateTime.Today:yyyy-MM-dd}.xlsx"
        };
        if (dialog.ShowDialog() != true) return;
        try
        {
            _viewModel.ExportExcel(dialog.FileName);
            ShowSuccess(dialog.FileName);
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private void ExportPdf_Click(object sender, RoutedEventArgs e)
    {
        if (!CanExport()) return;
        var dialog = new SaveFileDialog
        {
            Title = "Зберегти звіт про відвідуваність (PDF)",
            Filter = "Документ PDF (*.pdf)|*.pdf",
            DefaultExt = ".pdf",
            AddExtension = true,
            OverwritePrompt = true,
            FileName = $"Відвідуваність_{DateTime.Today:yyyy-MM-dd}.pdf"
        };
        if (dialog.ShowDialog() != true) return;
        try
        {
            _viewModel.ExportPdf(dialog.FileName);
            ShowSuccess(dialog.FileName);
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private bool CanExport()
    {
        if (_viewModel.IsLoading || _viewModel.Report.Groups.Count == 0)
        {
            MessageBox.Show("Дочекайтеся завантаження або виберіть групу з даними.",
                "EducationSystem", MessageBoxButton.OK, MessageBoxImage.Information);
            return false;
        }
        if (_viewModel.Report.GroupId != _viewModel.SelectedGroup?.Id ||
            _viewModel.Report.SubjectId != _viewModel.SelectedSubject?.Id ||
            _viewModel.Report.FromDate != _viewModel.FromDate?.Date ||
            _viewModel.Report.ToDate != _viewModel.ToDate?.Date)
        {
            MessageBox.Show("Спочатку натисніть «Показати», щоб застосувати фільтри.",
                "EducationSystem", MessageBoxButton.OK, MessageBoxImage.Information);
            return false;
        }
        return true;
    }

    private static void ShowSuccess(string path) => MessageBox.Show(
        $"Звіт збережено:\n{path}", "EducationSystem",
        MessageBoxButton.OK, MessageBoxImage.Information);

    private static void ShowError(Exception e) => MessageBox.Show(
        $"Не вдалося зберегти звіт:\n{e.Message}", "Помилка експорту",
        MessageBoxButton.OK, MessageBoxImage.Error);
}
