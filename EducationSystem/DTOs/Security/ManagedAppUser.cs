namespace EducationSystem.DTOs.Security;

public sealed class ManagedAppUser
{
    public int Id { get; set; }
    public string LoginName { get; set; } = string.Empty;
    public string AccountRole { get; set; } = string.Empty;
    public int? TeacherId { get; set; }
    public string TeacherFullName { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    public string RoleName => AccountRole == "teacher" ? "Викладач" : "Адміністрація";
    public string StatusName => IsActive ? "Активний" : "Заблокований";
}

public sealed class ManagedTeacherOption
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
}
