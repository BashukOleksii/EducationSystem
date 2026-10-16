
using Dapper;
using EducationSystem.Data;
using EducationSystem.DTOs.Analytics;

namespace EducationSystem.Repositories;

public sealed class LearningDynamicsRepository
{
    private readonly DatabaseConnectionFactory _connectionFactory;

    public LearningDynamicsRepository(
        DatabaseConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<LearningDynamicsReportData> GetReportAsync(
        DateTime? fromDate,
        DateTime? toDate)
    {
        using var db = _connectionFactory.CreateQueryFactory();

        var parameters = new
        {
            FromDate = fromDate?.Date,
            ToExclusive = toDate?.Date.AddDays(1)
        };

        const string sql = """
            SET NOCOUNT ON;

            -- Формуємо спільний набір занять.
            SELECT
                l.id AS LessonId,
                l.[date] AS LessonDate,
                l.group_id AS GroupId,
                ts.subject_id AS SubjectId,
                ts.teacher_id AS TeacherId
            INTO #LessonFacts
            FROM dbo.lessons AS l
            INNER JOIN dbo.teachers_subjects AS ts
                ON ts.id = l.teacher_id
            WHERE (@FromDate IS NULL OR l.[date] >= @FromDate)
              AND (@ToExclusive IS NULL OR l.[date] < @ToExclusive);

            -- 1. Динаміка за місяцями.
            SELECT
                CONVERT(char(7), LessonDate, 120) AS MonthKey,
                COUNT(*) AS LessonsCount,
                COUNT(DISTINCT GroupId) AS GroupCount
            FROM #LessonFacts
            GROUP BY CONVERT(char(7), LessonDate, 120)
            ORDER BY MonthKey;

            -- 2. Розподіл занять за предметами.
            SELECT
                s.name AS SubjectName,
                COUNT(*) AS LessonsCount,
                COUNT(DISTINCT f.GroupId) AS GroupCount
            FROM #LessonFacts AS f
            INNER JOIN dbo.subjects AS s
                ON s.id = f.SubjectId
            GROUP BY s.id, s.name
            ORDER BY LessonsCount DESC, s.name;

            -- 3. Навантаження навчальних груп.
            SELECT
                CONCAT(g.prefix, '-', g.number) AS GroupName,
                COUNT(*) AS LessonsCount,
                COUNT(DISTINCT f.SubjectId) AS SubjectCount
            FROM #LessonFacts AS f
            INNER JOIN dbo.groups AS g
                ON g.id = f.GroupId
            GROUP BY g.id, g.prefix, g.number
            ORDER BY LessonsCount DESC, GroupName;

            -- 4. Загальні підсумки.
            SELECT
                COUNT(*) AS TotalLessons,
                COUNT(DISTINCT GroupId) AS TotalGroups,
                COUNT(DISTINCT SubjectId) AS TotalSubjects,
                COUNT(DISTINCT TeacherId) AS TotalTeachers
            FROM #LessonFacts;
            """;

        using var reader = await db.Connection.QueryMultipleAsync(
            sql,
            parameters);

        var months = (await reader
            .ReadAsync<LearningDynamicsRow>()).ToList();

        var subjects = (await reader
            .ReadAsync<SubjectLessonsRow>()).ToList();

        var groups = (await reader
            .ReadAsync<GroupLessonsRow>()).ToList();

        var summary = await reader
            .ReadSingleAsync<SummaryTotalsRow>();

        return new LearningDynamicsReportData
        {
            MonthlyLessons = months,
            SubjectLessons = subjects,
            GroupLessons = groups,
            Summary = summary
        };
    }
}
