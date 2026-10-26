
using Microsoft.Data.SqlClient;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace EducationSystem.Data;

public sealed class DatabaseConnectionFactory
{
    private readonly string _connectionString;
    private readonly SqlCredential _credential;

    public DatabaseConnectionFactory(
        string connectionString,
        SqlCredential credential)
    {
        _connectionString = connectionString;
        _credential = credential;
    }

    public SqlConnection CreateConnection()
    {
        return new SqlConnection(
            _connectionString,
            _credential
        );
    }

    public QueryFactory CreateQueryFactory()
    {
        return new QueryFactory(
            CreateConnection(),
            new SqlServerCompiler()
        );
    }
}
