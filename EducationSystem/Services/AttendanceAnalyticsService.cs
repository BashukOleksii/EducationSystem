using EducationSystem.DTOs.Analytics;
using EducationSystem.Repositories.Interfaces;

namespace EducationSystem.Services;

public sealed class AttendanceAnalyticsService
{
    private readonly IAttendanceAnalyticsRepository _repository;

    public AttendanceAnalyticsService(IAttendanceAnalyticsRepository repository)
    {
        _repository = repository;
    }

    public async Task<AttendanceReportData> GetAsync(
        int? groupId, int? subjectId,
        DateTime? fromDate, DateTime? toDate,
        string? groupName, string? subjectName)
    {
        if (groupId is <= 0 || subjectId is <= 0)
            throw new ArgumentException("Ідентифікатор групи чи предмета має бути більшим за нуль.");
        if (fromDate.HasValue && toDate.HasValue && fromDate.Value.Date > toDate.Value.Date)
            throw new ArgumentException("Початкова дата не може бути пізнішою за кінцеву.");
        if (toDate?.Date == DateTime.MaxValue.Date)
            throw new ArgumentException("Оберіть кінцеву дату раніше за 31.12.9999.");

        AttendanceReportData result = await _repository.GetAsync(groupId, subjectId, fromDate, toDate);
        result.GroupLabel = groupId.HasValue ? groupName ?? $"Група №{groupId}" : "Усі групи";
        result.SubjectLabel = subjectId.HasValue ? subjectName ?? $"Предмет №{subjectId}" : "Усі предмети";
        return result;
    }
}
