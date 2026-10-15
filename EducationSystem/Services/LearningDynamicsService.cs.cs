
using EducationSystem.DTOs.Analytics;
using EducationSystem.Repositories;

namespace EducationSystem.Services;

public sealed class LearningDynamicsService
{
    private readonly LearningDynamicsRepository _repository;

    public LearningDynamicsService(
        LearningDynamicsRepository repository)
    {
        _repository = repository;
    }

    public async Task<LearningDynamicsReportData> GetReportAsync(
        DateTime? fromDate,
        DateTime? toDate)
    {
        if (fromDate.HasValue &&
            toDate.HasValue &&
            fromDate.Value.Date > toDate.Value.Date)
        {
            throw new ArgumentException(
                "Початкова дата не може бути пізнішою за кінцеву.");
        }

        if (toDate?.Date == DateTime.MaxValue.Date)
        {
            throw new ArgumentException(
                "Некоректна кінцева дата.");
        }

        var report = await _repository.GetReportAsync(
            fromDate,
            toDate);

        if (fromDate == null && toDate == null)
        {
            report.Summary.PeriodLabel = "За весь період";
        }
        else
        {
            string start = fromDate.HasValue
                ? fromDate.Value.ToString("dd.MM.yyyy")
                : "початку обліку";

            string end = toDate.HasValue
                ? toDate.Value.ToString("dd.MM.yyyy")
                : "сьогодні";

            report.Summary.PeriodLabel =
                $"Період: {start} — {end}";
        }

        return report;
    }
}
