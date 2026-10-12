using Dapper;
using EducationSystem.Data;
using EducationSystem.DTOs.Analytics;
using EducationSystem.Repositories.Interfaces;
using SqlKata.Execution;

namespace EducationSystem.Repositories;

public sealed class AttendanceAnalyticsRepository : IAttendanceAnalyticsRepository
{
    private readonly DatabaseConnectionFactory _connectionFactory;

    public AttendanceAnalyticsRepository(DatabaseConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<AttendanceReportData> GetAsync(
        int? groupId, int? subjectId, DateTime? fromDate, DateTime? toDate)
    {
        using QueryFactory db = _connectionFactory.CreateQueryFactory();
        var parameters = new
        {
            GroupId = groupId,
            SubjectId = subjectId,
            FromDate = fromDate?.Date,
            ToExclusive = toDate?.Date.AddDays(1)
        };
        const string sql = """
            SET NOCOUNT ON;

            SELECT l.group_id AS GroupId,
                   ts.subject_id AS SubjectId,
                   s.id AS StudentId,
                   l.[date] AS LessonDate,
                   CASE WHEN a.status = 'present' THEN 1 ELSE 0 END AS PresentValue,
                   CASE WHEN a.status = 'missing' THEN 1 ELSE 0 END AS MissingValue,
                   CASE WHEN a.status IS NULL OR a.status NOT IN ('present', 'missing')
                        THEN 1 ELSE 0 END AS UnmarkedValue
            INTO #AttendancePairs
            FROM lessons l
            INNER JOIN groups g ON g.id = l.group_id AND g.is_active = 1
            INNER JOIN teachers_subjects ts ON ts.id = l.teacher_id
            INNER JOIN students s ON s.group_id = g.id
            OUTER APPLY
            (
                SELECT TOP (1) entry.status
                FROM attendance entry
                WHERE entry.lesson_id = l.id AND entry.student_id = s.id
                ORDER BY entry.id DESC
            ) a
            WHERE ts.subgroup IS NULL
              AND (@GroupId IS NULL OR l.group_id = @GroupId)
              AND (@SubjectId IS NULL OR ts.subject_id = @SubjectId)
              AND (@FromDate IS NULL OR l.[date] >= @FromDate)
              AND (@ToExclusive IS NULL OR l.[date] < @ToExclusive);

            -- Підсумок за групами, включаючи активні групи без занять/студентів.
            SELECT g.id AS GroupId,
                   CONCAT(g.prefix, '-', g.number) AS GroupName,
                   COALESCE(sc.StudentCount, 0) AS StudentCount,
                   COALESCE(p.ExpectedCount, 0) AS ExpectedCount,
                   COALESCE(p.PresentCount, 0) AS PresentCount,
                   COALESCE(p.MissingCount, 0) AS MissingCount,
                   COALESCE(p.UnmarkedCount, 0) AS UnmarkedCount
            FROM groups g
            LEFT JOIN
            (
                SELECT group_id, COUNT(*) AS StudentCount
                FROM students GROUP BY group_id
            ) sc ON sc.group_id = g.id
            LEFT JOIN
            (
                SELECT GroupId,
                       COUNT(*) AS ExpectedCount,
                       SUM(PresentValue) AS PresentCount,
                       SUM(MissingValue) AS MissingCount,
                       SUM(UnmarkedValue) AS UnmarkedCount
                FROM #AttendancePairs GROUP BY GroupId
            ) p ON p.GroupId = g.id
            WHERE g.is_active = 1 AND (@GroupId IS NULL OR g.id = @GroupId)
            ORDER BY g.prefix, g.number;

            -- Підсумок для кожного поточного студента активної групи.
            SELECT s.id AS StudentId,
                   g.id AS GroupId,
                   CONCAT(g.prefix, '-', g.number) AS GroupName,
                   CONCAT(s.last_name, ' ', s.first_name) AS FullName,
                   COALESCE(p.ExpectedCount, 0) AS ExpectedCount,
                   COALESCE(p.PresentCount, 0) AS PresentCount,
                   COALESCE(p.MissingCount, 0) AS MissingCount,
                   COALESCE(p.UnmarkedCount, 0) AS UnmarkedCount
            FROM students s
            INNER JOIN groups g ON g.id = s.group_id AND g.is_active = 1
            LEFT JOIN
            (
                SELECT StudentId,
                       COUNT(*) AS ExpectedCount,
                       SUM(PresentValue) AS PresentCount,
                       SUM(MissingValue) AS MissingCount,
                       SUM(UnmarkedValue) AS UnmarkedCount
                FROM #AttendancePairs GROUP BY StudentId
            ) p ON p.StudentId = s.id
            WHERE (@GroupId IS NULL OR s.group_id = @GroupId)
            ORDER BY g.prefix, g.number, s.last_name, s.first_name;

            -- Порівняння дисциплін за відвідуваністю та заповненістю журналу.
            SELECT su.id AS SubjectId,
                   su.name AS SubjectName,
                   COALESCE(p.ExpectedCount, 0) AS ExpectedCount,
                   COALESCE(p.PresentCount, 0) AS PresentCount,
                   COALESCE(p.MissingCount, 0) AS MissingCount,
                   COALESCE(p.UnmarkedCount, 0) AS UnmarkedCount
            FROM subjects su
            LEFT JOIN
            (
                SELECT SubjectId,
                       COUNT(*) AS ExpectedCount,
                       SUM(PresentValue) AS PresentCount,
                       SUM(MissingValue) AS MissingCount,
                       SUM(UnmarkedValue) AS UnmarkedCount
                FROM #AttendancePairs GROUP BY SubjectId
            ) p ON p.SubjectId = su.id
            WHERE su.is_active = 1
              AND (@SubjectId IS NULL OR su.id = @SubjectId)
            ORDER BY su.name;

            -- Помісячна динаміка. Ключ yyyy-MM правильно сортується як рядок.
            SELECT CONVERT(char(7), LessonDate, 120) AS MonthKey,
                   COUNT(*) AS ExpectedCount,
                   SUM(PresentValue) AS PresentCount,
                   SUM(MissingValue) AS MissingCount,
                   SUM(UnmarkedValue) AS UnmarkedCount
            FROM #AttendancePairs
            GROUP BY CONVERT(char(7), LessonDate, 120)
            ORDER BY MonthKey;
            """;

        using var reader = await db.Connection.QueryMultipleAsync(sql, parameters);
        var groups = (await reader.ReadAsync<GroupAttendanceRow>()).ToList();
        var students = (await reader.ReadAsync<StudentAttendanceRow>()).ToList();
        var subjects = (await reader.ReadAsync<SubjectAttendanceRow>()).ToList();
        var months = (await reader.ReadAsync<MonthAttendanceRow>()).ToList();

        return new AttendanceReportData
        {
            FromDate = fromDate?.Date,
            ToDate = toDate?.Date,
            GroupId = groupId,
            SubjectId = subjectId,
            Groups = groups,
            Students = students,
            Subjects = subjects,
            Months = months
        };
    }
}
