using EducationSystem.ViewModels;
using EducationSystem.Visualizations;
using Microsoft.Win32;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace EducationSystem.Views;

public partial class GroupEnrollmentView : UserControl
{
    private readonly AnalyticsViewModel _viewModel;

    public GroupEnrollmentView(AnalyticsViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;

        Loaded += GroupEnrollmentView_Loaded;
        Unloaded += GroupEnrollmentView_Unloaded;
    }

    private void GroupEnrollmentView_Loaded(object sender, RoutedEventArgs e)
    {
        _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        DrawChart();
    }

    private void GroupEnrollmentView_Unloaded(object sender, RoutedEventArgs e)
    {
        _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AnalyticsViewModel.IsLoading)
            && !_viewModel.IsLoading)
        {
            DrawChart();
        }
    }

    private void DrawChart()
    {
        GroupEnrollmentChart.Populate(
            GroupPlot.Plot,
            _viewModel.GroupRows.ToList());

        GroupPlot.Refresh();
    }

    private async void Refresh_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.IsLoading)
            return;

        await _viewModel.LoadAsync();
    }

    private void ExportExcel_Click(object sender, RoutedEventArgs e)
    {
        if (!CanExport())
            return;

        var dialog = new SaveFileDialog
        {
            Title = "Зберегти звіт про контингент (Excel)",
            Filter = "Книга Excel (*.xlsx)|*.xlsx",
            DefaultExt = ".xlsx",
            AddExtension = true,
            OverwritePrompt = true,
            FileName = $"Контингент_{DateTime.Today:yyyy-MM-dd}.xlsx"
        };

        if (dialog.ShowDialog() != true)
            return;

        try
        {
            _viewModel.ExportExcel(dialog.FileName);
            ShowSuccess(dialog.FileName);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void ExportPdf_Click(object sender, RoutedEventArgs e)
    {
        if (!CanExport())
            return;

        var dialog = new SaveFileDialog
        {
            Title = "Зберегти звіт про контингент (PDF)",
            Filter = "Документ PDF (*.pdf)|*.pdf",
            DefaultExt = ".pdf",
            AddExtension = true,
            OverwritePrompt = true,
            FileName = $"Контингент_{DateTime.Today:yyyy-MM-dd}.pdf"
        };

        if (dialog.ShowDialog() != true)
            return;

        try
        {
            _viewModel.ExportPdf(dialog.FileName);
            ShowSuccess(dialog.FileName);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private bool CanExport()
    {
        if (_viewModel.IsLoading)
        {
            MessageBox.Show(
                "Спочатку дочекайтеся завершення завантаження даних.",
                "EducationSystem",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            return false;
        }

        if (_viewModel.GroupRows.Count == 0)
        {
            MessageBox.Show(
                "Немає активних груп для формування звіту.",
                "EducationSystem",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            return false;
        }

        return true;
    }

    private static void ShowSuccess(string path)
    {
        MessageBox.Show(
            $"Звіт успішно збережено:\n{path}",
            "EducationSystem",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private static void ShowError(Exception exception)
    {
        MessageBox.Show(
            $"Не вдалося сформувати звіт:\n{exception.Message}",
            "Помилка експорту",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }
}
