using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EducationSystem.DTOs.Analytics;
using EducationSystem.Models;
using EducationSystem.Services;
using System.Collections.ObjectModel;

namespace EducationSystem.ViewModels;

public partial class PerformanceViewModel : ObservableObject
{
    private readonly PerformanceService _service;
    private readonly GroupService _groupService;
    private readonly PerformanceReportExportService _exporter;

    public PerformanceViewModel(
        PerformanceService service,
        GroupService groupService,
        PerformanceReportExportService exporter)
    {
        _service = service;
        _groupService = groupService;
        _exporter = exporter;
    }

    public ObservableCollection<Group> Groups { get; } = new();

    [ObservableProperty]
    private Group? selectedGroup;

    [ObservableProperty]
    private DateTime? fromDate;

    [ObservableProperty]
    private DateTime? toDate;

    [ObservableProperty]
    private PerformanceReportData report = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string message = string.Empty;

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsLoading)
            return;

        try
        {
            IsLoading = true;
            Message = string.Empty;

            if (Groups.Count == 0)
            {
                var groups = await _groupService.GetAllAsync();
                foreach (var group in groups)
                    Groups.Add(group);
            }

            Report = await _service.GetAsync(
                SelectedGroup?.Id,
                FromDate,
                ToDate,
                SelectedGroup?.Name);
        }
        catch (Exception e)
        {
            Report = new PerformanceReportData();
            Message = e.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ClearFiltersAsync()
    {
        SelectedGroup = null;
        FromDate = null;
        ToDate = null;
        await LoadAsync();
    }

    public void ExportExcel(string filePath)
    {
        _exporter.ExportExcel(filePath, Report);
    }

    public void ExportPdf(string filePath)
    {
        _exporter.ExportPdf(filePath, Report);
    }
}
