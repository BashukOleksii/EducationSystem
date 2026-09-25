using Microsoft.Data.SqlClient;
using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Text;

namespace EducationSystem.Data
{
    public sealed class DatabaseConnectionFactory
    {
        private readonly string _connectionString;
        public DatabaseConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }
        
        public QueryFactory CreateQueryFactory()
        {
            return new QueryFactory(
                new SqlConnection(_connectionString),
                new SqlKata.Compilers.SqlServerCompiler()
            );
        }
    }
}
