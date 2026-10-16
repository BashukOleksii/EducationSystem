namespace EducationSystem.DTOs.Analytics;

public abstract class AttendanceTotals
{
    public int ExpectedCount { get; set; }
    public int PresentCount { get; set; }
    public int MissingCount { get; set; }
    public int UnmarkedCount { get; set; }
    public int MarkedCount => PresentCount + MissingCount;
    public double? AttendancePercent => MarkedCount == 0
        ? null
        : 100.0 * PresentCount / MarkedCount;
    public double? CoveragePercent => ExpectedCount == 0
        ? null
        : 100.0 * MarkedCount / ExpectedCount;
}

public sealed class GroupAttendanceRow : AttendanceTotals
{
    public int GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public int StudentCount { get; set; }
}

public sealed class StudentAttendanceRow : AttendanceTotals
{
    public int StudentId { get; set; }
    public int GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}

public sealed class SubjectAttendanceRow : AttendanceTotals
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
}

public sealed class MonthAttendanceRow : AttendanceTotals
{
    public string MonthKey { get; set; } = string.Empty;
}

public sealed class AttendanceReportData
{
    public int? GroupId { get; set; }
    public int? SubjectId { get; set; }
    public string GroupLabel { get; set; } = "Усі групи";
    public string SubjectLabel { get; set; } = "Усі предмети";
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public IReadOnlyList<GroupAttendanceRow> Groups { get; set; } = [];
    public IReadOnlyList<StudentAttendanceRow> Students { get; set; } = [];
    public IReadOnlyList<SubjectAttendanceRow> Subjects { get; set; } = [];
    public IReadOnlyList<MonthAttendanceRow> Months { get; set; } = [];

    public int TotalStudents => Groups.Sum(x => x.StudentCount);
    public int TotalExpected => Groups.Sum(x => x.ExpectedCount);
    public int TotalPresent => Groups.Sum(x => x.PresentCount);
    public int TotalMissing => Groups.Sum(x => x.MissingCount);
    public int TotalUnmarked => Groups.Sum(x => x.UnmarkedCount);
    public int TotalMarked => TotalPresent + TotalMissing;
    public double? AttendancePercent => TotalMarked == 0
        ? null
        : 100.0 * TotalPresent / TotalMarked;
    public double? CoveragePercent => TotalExpected == 0
        ? null
        : 100.0 * TotalMarked / TotalExpected;
    public string AttendanceLabel => FormatPercent(AttendancePercent);
    public string CoverageLabel => FormatPercent(CoveragePercent);
    public string PeriodLabel => FromDate is null && ToDate is null
        ? "Увесь період"
        : $"{(FromDate.HasValue ? FromDate.Value.ToString("dd.MM.yyyy") : "від початку")} — " +
          $"{(ToDate.HasValue ? ToDate.Value.ToString("dd.MM.yyyy") : "до сьогодні")}";

    public static string FormatPercent(double? percentage) =>
        percentage.HasValue ? $"{percentage.Value:0.0}%" : "—";
}
