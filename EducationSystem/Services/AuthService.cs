using EducationSystem.Data;
using EducationSystem.Security;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security;

namespace EducationSystem.Services;

public sealed class AuthService
{
    private readonly string _connectionString;

    public AuthService(string connectionString)
    {
        var builder = new SqlConnectionStringBuilder(connectionString);

        if (builder.IntegratedSecurity ||
            !string.IsNullOrEmpty(builder.UserID) ||
            !string.IsNullOrEmpty(builder.Password))
        {
            throw new InvalidOperationException(
                "Рядок підключення не повинен містити Windows Authentication, логін або пароль.");
        }

        _connectionString = builder.ConnectionString;
    }

    public async Task<LoginResult> LoginAsync(
        string login,
        SecureString password)
    {
        login = login?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(login) ||
            password is null ||
            password.Length == 0)
        {
            throw new ArgumentException("Введіть логін і пароль.");
        }

        SecureString passwordCopy = password.Copy();
        passwordCopy.MakeReadOnly();

        try
        {
            var credential = new SqlCredential(login, passwordCopy);

            await using var connection =
                new SqlConnection(_connectionString, credential);

            await connection.OpenAsync();

            UserRole role;

            const string roleSql = """
                SELECT
                    COALESCE(IS_ROLEMEMBER('role_admin'), 0),
                    COALESCE(IS_ROLEMEMBER('role_administration'), 0),
                    COALESCE(IS_ROLEMEMBER('role_teacher'), 0);
                """;

            using (var command = new SqlCommand(roleSql, connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                if (!await reader.ReadAsync())
                {
                    throw new UnauthorizedAccessException(
                        "Не вдалося визначити роль користувача.");
                }

                bool isAdmin = reader.GetInt32(0) == 1;
                bool isAdministration = reader.GetInt32(1) == 1;
                bool isTeacher = reader.GetInt32(2) == 1;

                int rolesCount =
                    (isAdmin ? 1 : 0) +
                    (isAdministration ? 1 : 0) +
                    (isTeacher ? 1 : 0);

                if (rolesCount != 1)
                {
                    throw new UnauthorizedAccessException(
                        "Користувач повинен належати рівно до однієї ролі EducationSystem.");
                }

                role = isAdmin
                    ? UserRole.Admin
                    : isAdministration
                        ? UserRole.Administration
                        : UserRole.Teacher;
            }

            int? teacherId = null;

            if (role == UserRole.Teacher)
            {
                using var command = new SqlCommand(
                    "dbo.usp_GetCurrentTeacherId", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                object? value = await command.ExecuteScalarAsync();

                if (value is not int id)
                {
                    throw new UnauthorizedAccessException(
                        "Для цього облікового запису не знайдено викладача.");
                }

                teacherId = id;
            }

            var session = new UserSession(login, role, teacherId);

            var factory = new DatabaseConnectionFactory(
                _connectionString,
                credential);

            return new LoginResult(session, factory);
        }
        catch
        {
            passwordCopy.Dispose();
            throw;
        }
    }
}
