

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EducationSystem.DTOs.Analytics;
using EducationSystem.Services;

namespace EducationSystem.ViewModels;

public partial class LearningDynamicsViewModel : ObservableObject
{
    private readonly LearningDynamicsService _service;

    public LearningDynamicsViewModel(
        LearningDynamicsService service)
    {
        _service = service;
    }

    [ObservableProperty]
    private DateTime? fromDate;

    [ObservableProperty]
    private DateTime? toDate;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string message = string.Empty;

    [ObservableProperty]
    private LearningDynamicsReportData report = new();

    [RelayCommand]
    public async Task LoadAsync()
    {
        try
        {
            IsLoading = true;
            Message = string.Empty;

            Report = await _service.GetReportAsync(
                FromDate,
                ToDate);

            Message = Report.Summary.TotalLessons == 0
                ? "За вибраний період немає занять."
                : "Звіт сформовано успішно.";
        }
        catch (Exception ex)
        {
            Message = $"Помилка: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void ClearFilters()
    {
        FromDate = null;
        ToDate = null;
    }
}
