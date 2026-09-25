using EducationSystem.Data;
using EducationSystem.DTOs.Common;
using EducationSystem.Models;
using EducationSystem.Repositories.Interfaces;
using SqlKata;
using SqlKata.Execution;

namespace EducationSystem.Repositories
{
    public sealed class SubjectRepository : ISubjectRepository
    {
        private readonly DatabaseConnectionFactory _connectionFactory;

        private const string TableName = "subjects";

        public SubjectRepository(
            DatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<PagedResult<Subject>> GetPagedAsync(
            string? search,
            int page,
            int pageSize)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();

            Query query =
                db.Query(TableName);

            if (!string.IsNullOrWhiteSpace(search))
            {
                string value = search.Trim();

                query.WhereContains(
                    "name",
                    value
                );
            }

            IEnumerable<int> countResult =
                await query
                    .Clone()
                    .AsCount()
                    .GetAsync<int>();

            int totalCount =
                countResult.FirstOrDefault();

            IEnumerable<Subject> subjects =
                await query
                    .Clone()
                    .Select(
                        "id",
                        "name",
                        "duration",
                        "created_at",
                        "updated_at"
                    )
                    .OrderBy("name")
                    .ForPage(
                        page,
                        pageSize
                    )
                    .GetAsync<Subject>();

            return new PagedResult<Subject>
            {
                Items = subjects.ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<Subject?> GetByIdAsync(
            int id)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();

            IEnumerable<Subject> subjects =
                await db
                    .Query(TableName)
                    .Where("id", id)
                    .Limit(1)
                    .GetAsync<Subject>();

            return subjects.FirstOrDefault();
        }

        public async Task<bool> ExistsByNameAsync(
            string name,
            int? excludeId = null)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();

            Query query =
                db.Query(TableName)
                    .Where("name", name);

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
            Subject subject)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();

            int id =
                await db
                    .Query(TableName)
                    .InsertGetIdAsync<int>(
                        new
                        {
                            name = subject.Name,
                            duration = subject.Duration
                        }
                    );

            return id;
        }

        public async Task UpdateAsync(
            Subject subject)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();

            await db
                .Query(TableName)
                .Where(
                    "id",
                    subject.Id
                )
                .UpdateAsync(
                    new
                    {
                        name = subject.Name,
                        duration = subject.Duration,
                        updated_at = DateTime.Now
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
                .Where("id", id)
                .DeleteAsync();
        }
    }
}