using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EducationSystem.DTOs.Analytics;
using EducationSystem.Models;
using EducationSystem.Services;
using System.Collections.ObjectModel;

namespace EducationSystem.ViewModels;

public partial class AttendanceViewModel : ObservableObject
{
    private readonly AttendanceAnalyticsService _service;
    private readonly GroupService _groupService;
    private readonly SubjectService _subjectService;
    private readonly AttendanceReportExportService _exporter;

    public AttendanceViewModel(
        AttendanceAnalyticsService service,
        GroupService groupService,
        SubjectService subjectService,
        AttendanceReportExportService exporter)
    {
        _service = service;
        _groupService = groupService;
        _subjectService = subjectService;
        _exporter = exporter;
    }

    public ObservableCollection<Group> Groups { get; } = new();
    public ObservableCollection<Subject> Subjects { get; } = new();

    [ObservableProperty]
    private Group? selectedGroup;

    [ObservableProperty]
    private Subject? selectedSubject;

    [ObservableProperty]
    private DateTime? fromDate;

    [ObservableProperty]
    private DateTime? toDate;

    [ObservableProperty]
    private AttendanceReportData report = new();

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

            if (Subjects.Count == 0)
            {
                var subjects = await _subjectService.GetAllAsync();
                foreach (var subject in subjects)
                    Subjects.Add(subject);
            }

            Report = await _service.GetAsync(
                SelectedGroup?.Id,
                SelectedSubject?.Id,
                FromDate,
                ToDate,
                SelectedGroup?.Name,
                SelectedSubject?.Name);
        }
        catch (Exception exception)
        {
            Report = new AttendanceReportData();
            Message = exception.Message;
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
        SelectedSubject = null;
        FromDate = null;
        ToDate = null;
        await LoadAsync();
    }

    public void ExportExcel(string filePath) => _exporter.ExportExcel(filePath, Report);
    public void ExportPdf(string filePath) => _exporter.ExportPdf(filePath, Report);
}
