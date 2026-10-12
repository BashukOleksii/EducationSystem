using EducationSystem.DTOs.Analytics;
using EducationSystem.Repositories.Interfaces;

namespace EducationSystem.Services;

public sealed class PerformanceService
{
    private readonly IPerformanceRepository _repository;

    public PerformanceService(IPerformanceRepository repository)
    {
        _repository = repository;
    }

    public async Task<PerformanceReportData> GetAsync(
        int? groupId,
        DateTime? fromDate,
        DateTime? toDate,
        string? groupName)
    {
        if (groupId.HasValue && groupId.Value <= 0)
            throw new ArgumentException("Некоректний ідентифікатор групи.");

        if (fromDate.HasValue && toDate.HasValue && fromDate.Value.Date > toDate.Value.Date)
            throw new ArgumentException("Початкова дата не може бути пізніше кінцевої.");

        PerformanceReportData data = await _repository.GetAsync(groupId, fromDate, toDate);
        data.GroupId = groupId;
        data.GroupLabel = groupId.HasValue ? groupName ?? $"Група №{groupId}" : "Усі групи";

        Dictionary<int, int> counts = data.Distribution.ToDictionary(x => x.Grade, x => x.Count);
        data.Distribution = Enumerable.Range(1, 12)
            .Select(grade => new GradeDistributionRow
            {
                Grade = grade,
                Count = counts.GetValueOrDefault(grade)
            })
            .ToList();

        return data;
    }
}
