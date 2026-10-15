
using EducationSystem.DTOs.Analytics;
using EducationSystem.ViewModels;
using Microsoft.Reporting.WinForms;
using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace EducationSystem.Views;

public partial class LearningDynamicsView :
    System.Windows.Controls.UserControl
{
    private readonly LearningDynamicsViewModel _viewModel;
    private readonly ReportViewer _viewer;

    private bool _loaded;
    private bool _reportReady;

    public LearningDynamicsView(
        LearningDynamicsViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;

        _viewer = new ReportViewer
        {
            ProcessingMode = ProcessingMode.Local,
            Dock = System.Windows.Forms.DockStyle.Fill
        };

        ReportHost.Child = _viewer;

        _viewModel.PropertyChanged += OnPropertyChanged;

        Loaded += LearningDynamicsView_Loaded;
    }

    private async void LearningDynamicsView_Loaded(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        if (_loaded)
            return;

        _loaded = true;
        await _viewModel.LoadAsync();
    }


    private void OnPropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        if (e.PropertyName ==
            nameof(LearningDynamicsViewModel.Report))
        {
            RefreshReport();
        }
    }

    private void RefreshReport()
    {
        _reportReady = false;

        string path = Path.Combine(
            AppContext.BaseDirectory,
            "Reports",
            "LearningDynamicsReport.rdlc");

        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                "Не знайдено RDLC-шаблон.",
                path);
        }

        _viewer.LocalReport.ReportPath = path;

        _viewer.LocalReport.DataSources.Clear();

        LearningDynamicsReportData report = _viewModel.Report;

        // 1. Статистика за місяцями
        _viewer.LocalReport.DataSources.Add(
            new ReportDataSource(
                "LearningDynamicsData",
                report.MonthlyLessons.ToList()));

        // 2. Статистика за дисциплінами
        _viewer.LocalReport.DataSources.Add(
            new ReportDataSource(
                "SubjectLessonsData",
                report.SubjectLessons.ToList()));

        // 3. Статистика за групами
        _viewer.LocalReport.DataSources.Add(
            new ReportDataSource(
                "GroupLessonsData",
                report.GroupLessons.ToList()));

        // 4. Загальні підсумки
        _viewer.LocalReport.DataSources.Add(
            new ReportDataSource(
                "SummaryTotalsData",
                new[] { report.Summary }));

        _viewer.RefreshReport();

        _reportReady = true;
    }
    

    private void ExportPdf_Click(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        ExportReport(
            "PDF",
            "PDF (*.pdf)|*.pdf",
            "pdf");
    }

    private void ExportExcel_Click(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        ExportReport(
            "EXCELOPENXML",
            "Excel (*.xlsx)|*.xlsx",
            "xlsx");
    }

    private void ExportReport(
        string format,
        string filter,
        string extension)
    {
        if (!_reportReady)
        {
            System.Windows.MessageBox.Show(
                "Спочатку сформуйте звіт.");
            return;
        }

        var dialog = new SaveFileDialog
        {
            Title = "Зберегти звіт",
            Filter = filter,
            DefaultExt = extension,
            AddExtension = true,
            FileName = "LearningDynamics"
        };

        if (dialog.ShowDialog() != true)
            return;

        try
        {
            byte[] bytes =
                _viewer.LocalReport.Render(format);

            File.WriteAllBytes(
                dialog.FileName,
                bytes);

            System.Windows.MessageBox.Show(
                "Звіт успішно експортовано.");
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                ex.Message,
                "Помилка експорту");
        }
    }
}
