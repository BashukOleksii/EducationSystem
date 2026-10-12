namespace EducationSystem.DTOs.Analytics;

public sealed class GroupPerformanceRow
{
    public int GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public int StudentCount { get; set; }
    public int GradedStudents { get; set; }
    public int GradeCount { get; set; }
    public double? AverageGrade { get; set; }
    public int LowGradeCount { get; set; }
    public int HighGradeCount { get; set; }
    public int WithoutGrades => StudentCount - GradedStudents;
}

public sealed class StudentPerformanceRow
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public int GradeCount { get; set; }
    public double? AverageGrade { get; set; }
    public int LowGradeCount { get; set; }
    public int HighGradeCount { get; set; }
}

public sealed class GradeDistributionRow
{
    public int Grade { get; set; }
    public int Count { get; set; }
}

public sealed class PerformanceReportData
{
    public int? GroupId { get; set; }
    public string GroupLabel { get; set; } = "Усі групи";
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public IReadOnlyList<GroupPerformanceRow> Groups { get; set; } = [];
    public IReadOnlyList<StudentPerformanceRow> Students { get; set; } = [];
    public IReadOnlyList<GradeDistributionRow> Distribution { get; set; } = [];

    public int TotalStudents => Groups.Sum(g => g.StudentCount);
    public int GradedStudents => Groups.Sum(g => g.GradedStudents);
    public int TotalGrades => Groups.Sum(g => g.GradeCount);
    public int LowGradeCount => Groups.Sum(g => g.LowGradeCount);
    public int HighGradeCount => Groups.Sum(g => g.HighGradeCount);
    public double? AverageGrade => TotalGrades == 0
        ? null
        : Groups.Sum(g => g.AverageGrade.GetValueOrDefault() * g.GradeCount) / TotalGrades;

    public string PeriodLabel => FromDate is null && ToDate is null
        ? "Увесь період"
        : $"{(FromDate.HasValue ? FromDate.Value.ToString("dd.MM.yyyy") : "без початку")} — " +
          $"{(ToDate.HasValue ? ToDate.Value.ToString("dd.MM.yyyy") : "без кінця")}";
}
