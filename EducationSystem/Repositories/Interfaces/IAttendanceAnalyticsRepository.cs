using EducationSystem.DTOs.Analytics;

namespace EducationSystem.Repositories.Interfaces;

public interface IAttendanceAnalyticsRepository
{
    Task<AttendanceReportData> GetAsync(
        int? groupId,
        int? subjectId,
        DateTime? fromDate,
        DateTime? toDate);
}
