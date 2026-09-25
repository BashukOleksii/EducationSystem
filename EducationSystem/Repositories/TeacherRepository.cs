using EducationSystem.Data;
using EducationSystem.DTOs.Common;
using EducationSystem.Models;
using EducationSystem.Models.Enums;
using EducationSystem.Repositories.Interfaces;
using SqlKata;
using SqlKata.Execution;

namespace EducationSystem.Repositories
{
    public sealed class TeacherRepository : ITeacherRepository
    {
        private readonly DatabaseConnectionFactory _connectionFactory;

        private const string TableName = "teachers";


        public TeacherRepository(
            DatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory =
                connectionFactory;
        }


        public async Task<PagedResult<Teacher>> GetPagedAsync(
            string? search,
            TeacherCategory? category,
            int page,
            int pageSize)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();

            Query query =
                db.Query(TableName);


         
            if (!string.IsNullOrWhiteSpace(search))
            {
                string[] searchParts =
                    search
                        .Trim()
                        .Split(
                            ' ',
                            StringSplitOptions.RemoveEmptyEntries
                        );

                foreach (string part in searchParts)
                {
                    string value = part;

                    query.Where(q =>
                    {
                        q.WhereContains(
                            "first_name",
                            value
                        );

                        q.OrWhereContains(
                            "last_name",
                            value
                        );

                        q.OrWhereContains(
                            "email",
                            value
                        );

                        return q;
                    });
                }
            }

            if (category.HasValue)
            {
                query.Where(
                    "category",
                    category.Value.ToString()
                );
            }


            IEnumerable<int> countResult =
                await query
                    .Clone()
                    .AsCount()
                    .GetAsync<int>();

            int totalCount =
                countResult.FirstOrDefault();

            IEnumerable<Teacher> teachers =
                await query
                    .Clone()
                    .Select(
                        "id",
                        "first_name",
                        "last_name",
                        "email",
                        "category",
                        "created_at",
                        "updated_at"
                    )
                    .OrderBy("last_name")
                    .OrderBy("first_name")
                    .ForPage(
                        page,
                        pageSize
                    )
                    .GetAsync<Teacher>();


            return new PagedResult<Teacher>
            {
                Items =
                    teachers.ToList(),

                TotalCount =
                    totalCount,

                Page =
                    page,

                PageSize =
                    pageSize
            };
        }

        public async Task<IReadOnlyList<Teacher>> GetAllAsync()
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();

            IEnumerable<Teacher> teachers =
                await db
                    .Query(TableName)
                    .Select(
                        "id",
                        "first_name",
                        "last_name",
                        "email",
                        "category"
                    )
                    .OrderBy("last_name")
                    .OrderBy("first_name")
                    .GetAsync<Teacher>();

            return teachers.ToList();
        }
        public async Task<Teacher?> GetByIdAsync(
            int id)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();

            IEnumerable<Teacher> teachers =
                await db
                    .Query(TableName)
                    .Where(
                        "id",
                        id
                    )
                    .Limit(1)
                    .GetAsync<Teacher>();

            return teachers.FirstOrDefault();
        }


        public async Task<bool> ExistsByEmailAsync(
            string email,
            int? excludeId = null)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();

            Query query =
                db.Query(TableName)
                    .Where(
                        "email",
                        email
                    );

            if (excludeId.HasValue)
            {
                query.Where(
                    "id",
                    "<>",
                    excludeId.Value
                );
            }

            IEnumerable<int> ids =
                await query
                    .Select("id")
                    .Limit(1)
                    .GetAsync<int>();

            return ids.Any();
        }


        public async Task<int> CreateAsync(
            Teacher teacher)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();

            int id =
                await db
                    .Query(TableName)
                    .InsertGetIdAsync<int>(
                        new
                        {
                            first_name =
                                teacher.FirstName,

                            last_name =
                                teacher.LastName,

                            email =
                                teacher.Email,

                            category =
                                teacher.Category.ToString()
                        }
                    );

            return id;
        }


        public async Task UpdateAsync(
            Teacher teacher)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();

            await db
                .Query(TableName)
                .Where(
                    "id",
                    teacher.Id
                )
                .UpdateAsync(
                    new
                    {
                        first_name =
                            teacher.FirstName,

                        last_name =
                            teacher.LastName,

                        email =
                            teacher.Email,

                        category =
                            teacher.Category.ToString(),

                        updated_at =
                            DateTime.Now
                    }
                );
        }


        public async Task DeleteAsync(
            int id)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();

            await db
                .Query(TableName)
                .Where(
                    "id",
                    id
                )
                .DeleteAsync();
        }
    }
}