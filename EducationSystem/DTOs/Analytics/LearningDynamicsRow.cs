namespace EducationSystem.DTOs.Analytics;

public sealed class LearningDynamicsRow
{
    public string MonthKey { get; set; } = string.Empty;

    public int LessonsCount { get; set; }

    public int GroupCount { get; set; }
}