using EducationSystem.DTOs.Analytics;

namespace EducationSystem.Repositories.Interfaces;

public interface IPerformanceRepository
{
    Task<PerformanceReportData> GetAsync(
        int? groupId,
        DateTime? fromDate,
        DateTime? toDate);
}
