
namespace EducationSystem.DTOs.Analytics;

// Кількість занять за кожною дисципліною
public sealed class SubjectLessonsRow
{
    public string SubjectName { get; set; } = string.Empty;

    public int LessonsCount { get; set; }

    public int GroupCount { get; set; }
}

public sealed class GroupLessonsRow
{
    public string GroupName { get; set; } = string.Empty;

    public int LessonsCount { get; set; }

    public int SubjectCount { get; set; }
}

public sealed class SummaryTotalsRow
{
    public int TotalLessons { get; set; }

    public int TotalGroups { get; set; }

    public int TotalSubjects { get; set; }

    public int TotalTeachers { get; set; }

    public string PeriodLabel { get; set; } = string.Empty;
}

public sealed class LearningDynamicsReportData
{
    public IReadOnlyList<LearningDynamicsRow> MonthlyLessons
    { get; set; } = [];

    public IReadOnlyList<SubjectLessonsRow> SubjectLessons
    { get; set; } = [];

    public IReadOnlyList<GroupLessonsRow> GroupLessons
    { get; set; } = [];

    public SummaryTotalsRow Summary
    { get; set; } = new();
}
