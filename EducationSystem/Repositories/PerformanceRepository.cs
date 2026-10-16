using Dapper;
using EducationSystem.Data;
using EducationSystem.DTOs.Analytics;
using EducationSystem.Repositories.Interfaces;
using SqlKata.Execution;

namespace EducationSystem.Repositories;

public sealed class PerformanceRepository : IPerformanceRepository
{
    private readonly DatabaseConnectionFactory _connectionFactory;

    public PerformanceRepository(DatabaseConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PerformanceReportData> GetAsync(
        int? groupId,
        DateTime? fromDate,
        DateTime? toDate)
    {
        using QueryFactory db = _connectionFactory.CreateQueryFactory();

        var parameters = new
        {
            GroupId = groupId,
            FromDate = fromDate?.Date,
            ToExclusive = toDate?.Date.AddDays(1)
        };

        const string sql = """
            WITH StudentCounts AS
            (
                SELECT group_id, COUNT(*) AS StudentCount
                FROM students
                GROUP BY group_id
            ),
            GradeStats AS
            (
                SELECT l.group_id,
                       COUNT(*) AS GradeCount,
                       COUNT(DISTINCT gr.student_id) AS GradedStudents,
                       AVG(CAST(gr.grade AS float)) AS AverageGrade,
                       SUM(CASE WHEN gr.grade BETWEEN 1 AND 3 THEN 1 ELSE 0 END) AS LowGradeCount,
                       SUM(CASE WHEN gr.grade BETWEEN 10 AND 12 THEN 1 ELSE 0 END) AS HighGradeCount
                FROM grades gr
                INNER JOIN lessons l ON l.id = gr.assessment_id
                INNER JOIN students s ON s.id = gr.student_id AND s.group_id = l.group_id
                WHERE gr.grade BETWEEN 1 AND 12
                  AND (@FromDate IS NULL OR l.date >= @FromDate)
                  AND (@ToExclusive IS NULL OR l.date < @ToExclusive)
                  AND (@GroupId IS NULL OR l.group_id = @GroupId)
                GROUP BY l.group_id
            )
            SELECT g.id AS GroupId,
                   CONCAT(g.prefix, '-', g.number) AS GroupName,
                   COALESCE(sc.StudentCount, 0) AS StudentCount,
                   COALESCE(gs.GradedStudents, 0) AS GradedStudents,
                   COALESCE(gs.GradeCount, 0) AS GradeCount,
                   gs.AverageGrade,
                   COALESCE(gs.LowGradeCount, 0) AS LowGradeCount,
                   COALESCE(gs.HighGradeCount, 0) AS HighGradeCount
            FROM groups g
            LEFT JOIN StudentCounts sc ON sc.group_id = g.id
            LEFT JOIN GradeStats gs ON gs.group_id = g.id
            WHERE g.is_active = 1
              AND (@GroupId IS NULL OR g.id = @GroupId)
            ORDER BY g.prefix, g.number;

            WITH ValidGrades AS
            (
                SELECT gr.student_id, l.group_id, gr.grade
                FROM grades gr
                INNER JOIN lessons l ON l.id = gr.assessment_id
                WHERE gr.grade BETWEEN 1 AND 12
                  AND (@FromDate IS NULL OR l.date >= @FromDate)
                  AND (@ToExclusive IS NULL OR l.date < @ToExclusive)
            )
            SELECT s.id AS StudentId,
                   CONCAT(s.last_name, ' ', s.first_name) AS FullName,
                   CONCAT(g.prefix, '-', g.number) AS GroupName,
                   COUNT(v.grade) AS GradeCount,
                   AVG(CAST(v.grade AS float)) AS AverageGrade,
                   SUM(CASE WHEN v.grade BETWEEN 1 AND 3 THEN 1 ELSE 0 END) AS LowGradeCount,
                   SUM(CASE WHEN v.grade BETWEEN 10 AND 12 THEN 1 ELSE 0 END) AS HighGradeCount
            FROM students s
            INNER JOIN groups g ON g.id = s.group_id AND g.is_active = 1
            LEFT JOIN ValidGrades v ON v.student_id = s.id AND v.group_id = g.id
            WHERE (@GroupId IS NULL OR g.id = @GroupId)
            GROUP BY s.id, s.first_name, s.last_name, g.prefix, g.number
            ORDER BY g.prefix, g.number, s.last_name, s.first_name;

            SELECT CAST(gr.grade AS int) AS Grade,
                   COUNT(*) AS [Count]
            FROM grades gr
            INNER JOIN lessons l ON l.id = gr.assessment_id
            INNER JOIN students s ON s.id = gr.student_id AND s.group_id = l.group_id
            INNER JOIN groups g ON g.id = l.group_id AND g.is_active = 1
            WHERE gr.grade BETWEEN 1 AND 12
              AND (@FromDate IS NULL OR l.date >= @FromDate)
              AND (@ToExclusive IS NULL OR l.date < @ToExclusive)
              AND (@GroupId IS NULL OR l.group_id = @GroupId)
            GROUP BY gr.grade
            ORDER BY gr.grade;
            """;

        using var reader = await db.Connection.QueryMultipleAsync(sql, parameters);

        var groups = (await reader.ReadAsync<GroupPerformanceRow>()).ToList();
        var students = (await reader.ReadAsync<StudentPerformanceRow>()).ToList();
        var distribution = (await reader.ReadAsync<GradeDistributionRow>()).ToList();

        return new PerformanceReportData
        {
            FromDate = fromDate?.Date,
            ToDate = toDate?.Date,
            Groups = groups,
            Students = students,
            Distribution = distribution
        };
    }
}
