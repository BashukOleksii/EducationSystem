using Microsoft.Data.SqlClient;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;

namespace EducationSystem.Data;

public sealed class DatabaseConnectionFactory : IDisposable
{
    private readonly string _connectionString;
    private readonly SqlCredential _credential;
    private bool _disposed;

    public DatabaseConnectionFactory(
        string connectionString,
        SqlCredential credential)
    {
        _connectionString = connectionString;
        _credential = credential;
    }

    public SqlConnection CreateConnection()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return new SqlConnection(_connectionString, _credential);
    }

    public QueryFactory CreateQueryFactory()
    {
        return new QueryFactory(CreateConnection(), new SqlServerCompiler());
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        using (var connection = new SqlConnection(_connectionString, _credential))
        {
            SqlConnection.ClearPool(connection);
        }

        _credential.Password.Dispose();
    }
}
