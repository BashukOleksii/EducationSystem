using EducationSystem.Data;
using EducationSystem.Security;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

    namespace EducationSystem.Services;

    public sealed class AuthService
    {
        private readonly string _connectionString;

        public AuthService(string connectionString)
        {
            var builder =
                new SqlConnectionStringBuilder(connectionString);

            if (builder.IntegratedSecurity ||
                !string.IsNullOrEmpty(builder.UserID) ||
                !string.IsNullOrEmpty(builder.Password))
            {
                throw new InvalidOperationException(
                    "У рядку підключення не повинно бути " +
                    "Windows Authentication, логіна чи пароля."
                );
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
                throw new ArgumentException(
                    "Введіть логін і пароль."
                );
            }

            SecureString passwordCopy = password.Copy();
            passwordCopy.MakeReadOnly();

            try
            {
                SqlCredential credential =
                    new SqlCredential(login, passwordCopy);

                await using SqlConnection connection =
                    new SqlConnection(
                        _connectionString,
                        credential
                    );

                await connection.OpenAsync();

                const string sql = """
                SELECT
                    COALESCE(
                        IS_ROLEMEMBER('role_admin'), 0
                    ),
                    COALESCE(
                        IS_ROLEMEMBER('role_administration'), 0
                    ),
                    COALESCE(
                        IS_ROLEMEMBER('role_teacher'), 0
                    );
                """;

                using SqlCommand command =
                    new SqlCommand(sql, connection);

                using var reader =
                    await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    throw new UnauthorizedAccessException(
                        "Не вдалося визначити роль користувача."
                    );
                }

                bool isAdmin = reader.GetInt32(0) == 1;

                bool isAdministration =
                    reader.GetInt32(1) == 1;

                bool isTeacher = reader.GetInt32(2) == 1;

                int roleCount =
                    (isAdmin ? 1 : 0) +
                    (isAdministration ? 1 : 0) +
                    (isTeacher ? 1 : 0);

                if (roleCount != 1)
                {
                    throw new UnauthorizedAccessException(
                        "Користувач повинен мати рівно одну " +
                        "роль EducationSystem."
                    );
                }

                UserRole role;

                if (isAdmin)
                    role = UserRole.Admin;
                else if (isAdministration)
                    role = UserRole.Administration;
                else
                    role = UserRole.Teacher;

                UserSession session =
                    new UserSession(login, role);

                DatabaseConnectionFactory factory =
                    new DatabaseConnectionFactory(
                        _connectionString,
                        credential
                    );

                return new LoginResult(
                    session,
                    factory
                );
            }
            catch
            {
                passwordCopy.Dispose();
                throw;
            }
        }
    }
