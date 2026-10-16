namespace EducationSystem.DTOs.Analytics;

public sealed class GroupEnrollmentRow
{
    public int Id { get; set; }
    public string Prefix { get; set; } = string.Empty;
    public byte Number { get; set; }
    public int StudentCount { get; set; }
    public double SharePercent { get; set; }
    public string GroupName => $"{Prefix}-{Number}";
}
