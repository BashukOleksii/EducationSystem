using Dapper;
using EducationSystem.Data;
using EducationSystem.DTOs.Security;
using System.Data;
using System.Text.RegularExpressions;

namespace EducationSystem.Services;

public sealed class ManagedAppUserService
{
    private readonly DatabaseConnectionFactory _connectionFactory;
    private static readonly Regex LoginPattern = new(
        @"^es_[A-Za-z0-9_]{3,67}$", RegexOptions.CultureInvariant);

    public ManagedAppUserService(DatabaseConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<ManagedAppUser>> GetUsersAsync()
    {
        await using var db = _connectionFactory.CreateConnection();
        var result = await db.QueryAsync<ManagedAppUser>(
            "dbo.usp_ListManagedAppUsers",
            commandType: CommandType.StoredProcedure);
        return result.AsList();
    }

    public async Task<IReadOnlyList<ManagedTeacherOption>> GetTeachersAsync()
    {
        await using var db = _connectionFactory.CreateConnection();
        var result = await db.QueryAsync<ManagedTeacherOption>(
            "dbo.usp_ListManagedTeachers",
            commandType: CommandType.StoredProcedure);
        return result.AsList();
    }

    public async Task CreateAsync(
        string loginName, string password, string role, int? teacherId)
    {
        ValidateAccount(loginName, role, teacherId);
        ValidatePassword(password);
        await using var db = _connectionFactory.CreateConnection();
        await db.ExecuteAsync(
            "dbo.usp_CreateManagedAppUser",
            new { LoginName = loginName, Password = password, AccountRole = role, TeacherId = teacherId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task ChangeAssignmentAsync(
        string loginName, string role, int? teacherId)
    {
        ValidateAccount(loginName, role, teacherId);
        await using var db = _connectionFactory.CreateConnection();
        await db.ExecuteAsync(
            "dbo.usp_UpdateManagedAppUser",
            new { LoginName = loginName, AccountRole = role, TeacherId = teacherId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task SetActiveAsync(string loginName, bool active)
    {
        ValidateLogin(loginName);
        await using var db = _connectionFactory.CreateConnection();
        await db.ExecuteAsync(
            "dbo.usp_SetManagedAppUserActive",
            new { LoginName = loginName, IsActive = active },
            commandType: CommandType.StoredProcedure);
    }

    public async Task ResetPasswordAsync(string loginName, string newPassword)
    {
        ValidateLogin(loginName);
        ValidatePassword(newPassword);
        await using var db = _connectionFactory.CreateConnection();
        await db.ExecuteAsync(
            "dbo.usp_ResetManagedAppUserPassword",
            new { LoginName = loginName, NewPassword = newPassword },
            commandType: CommandType.StoredProcedure);
    }

    public async Task DeleteAsync(string loginName)
    {
        ValidateLogin(loginName);
        await using var db = _connectionFactory.CreateConnection();
        await db.ExecuteAsync(
            "dbo.usp_DeleteManagedAppUser",
            new { LoginName = loginName },
            commandType: CommandType.StoredProcedure);
    }

    private static void ValidateAccount(string loginName, string role, int? teacherId)
    {
        ValidateLogin(loginName);
        if (role != "teacher" && role != "administration")
            throw new ArgumentException("Можна призначити лише роль викладача або адміністрації.");
        if (role == "teacher" && !teacherId.HasValue)
            throw new ArgumentException("Оберіть викладача для облікового запису.");
        if (role == "administration" && teacherId.HasValue)
            throw new ArgumentException("Роль адміністрації не повинна мати TeacherId.");
    }

    private static void ValidateLogin(string loginName)
    {
        if (string.IsNullOrEmpty(loginName) || !LoginPattern.IsMatch(loginName) || loginName.EndsWith('_'))
            throw new ArgumentException("Логін повинен починатися з es_ і містити 3–67 символів [A-Z, a-z, 0-9, _].");
    }

    private static void ValidatePassword(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 12 || password.Length > 128 || password.Contains('\''))
            throw new ArgumentException("Пароль має містити 12–128 символів без одинарних лапок.");
    }
}
