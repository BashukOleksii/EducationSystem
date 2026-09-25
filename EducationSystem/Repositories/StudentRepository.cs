using EducationSystem.Data;
using EducationSystem.DTOs.Common;
using EducationSystem.DTOs.Students;
using EducationSystem.Models;
using EducationSystem.Repositories.Interfaces;
using SqlKata;
using SqlKata.Execution;

namespace EducationSystem.Repositories
{
    public sealed class StudentRepository : IStudentRepository
    {
        private readonly DatabaseConnectionFactory _connectionFactory;

        private const string TableName = "students";


        public StudentRepository(
            DatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory =
                connectionFactory;
        }


        public async Task<PagedResult<StudentListItem>> GetPagedAsync(
            string? search,
            int? groupId,
            int page,
            int pageSize)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();


            Query query =
                db.Query(TableName)
                    .Join(
                        "groups",
                        "groups.id",
                        "students.group_id"
                    );

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
                            "students.first_name",
                            value
                        );

                        q.OrWhereContains(
                            "students.last_name",
                            value
                        );

                        q.OrWhereContains(
                            "students.email",
                            value
                        );

                        return q;
                    });
                }
            }

            if (groupId.HasValue)
            {
                query.Where(
                    "students.group_id",
                    groupId.Value
                );
            }

            IEnumerable<int> countResult =
                await query
                    .Clone()
                    .AsCount()
                    .GetAsync<int>();


            int totalCount =
                countResult.FirstOrDefault();


            IEnumerable<StudentListItem> students =
                await query
                    .Clone()
                    .Select(
                        "students.id",
                        "students.first_name",
                        "students.last_name",
                        "students.email",
                        "students.group_id",
                        "groups.prefix as group_prefix",
                        "groups.number as group_number"
                    )
                    .OrderBy("students.last_name")
                    .OrderBy("students.first_name")
                    .ForPage(
                        page,
                        pageSize
                    )
                    .GetAsync<StudentListItem>();


            return new PagedResult<StudentListItem>
            {
                Items =
                    students.ToList(),

                TotalCount =
                    totalCount,

                Page =
                    page,

                PageSize =
                    pageSize
            };
        }


        public async Task<Student?> GetByIdAsync(
            int id)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();


            IEnumerable<Student> students =
                await db
                    .Query(TableName)
                    .Where(
                        "id",
                        id
                    )
                    .Select(
                        "id",
                        "first_name",
                        "last_name",
                        "email",
                        "group_id"
                    )
                    .Limit(1)
                    .GetAsync<Student>();


            return students.FirstOrDefault();
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
            Student student)
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
                                student.FirstName,

                            last_name =
                                student.LastName,

                            email =
                                student.Email,

                            group_id =
                                student.GroupId
                        }
                    );


            return id;
        }


        public async Task UpdateAsync(
            Student student)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();


            await db
                .Query(TableName)
                .Where(
                    "id",
                    student.Id
                )
                .UpdateAsync(
                    new
                    {
                        first_name =
                            student.FirstName,

                        last_name =
                            student.LastName,

                        email =
                            student.Email,

                        group_id =
                            student.GroupId,

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