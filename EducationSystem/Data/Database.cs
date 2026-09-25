using Microsoft.Data.SqlClient;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Text;

namespace EducationSystem.Data
{
    public class Database
    {
        private readonly string _connectionString;

        public Database(string connectionString)
        {
            _connectionString = connectionString;
        }

        public QueryFactory CreateQueryFactory()
        {
            var connection = new SqlConnection(_connectionString);
            var compiler = new SqlServerCompiler();
            return new QueryFactory(connection, compiler);
        }
    }
}
