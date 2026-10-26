
using EducationSystem.Data;

namespace EducationSystem.Security;

public enum UserRole
{
    Teacher,
    Administration,
    Admin
}

public sealed record UserSession(
    string Login,
    UserRole Role
);

public sealed record LoginResult(
    UserSession User,
    DatabaseConnectionFactory DatabaseFactory
);
