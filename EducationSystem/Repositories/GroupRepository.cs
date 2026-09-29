using EducationSystem.Data;
using EducationSystem.DTOs.Common;
using EducationSystem.Models;
using EducationSystem.Repositories.Interfaces;
using SqlKata;
using SqlKata.Execution;

namespace EducationSystem.Repositories
{
    public sealed class GroupRepository : IGroupRepository
    {
        private readonly DatabaseConnectionFactory _connectionFactory;
        private const string TableName = "groups";

        public GroupRepository(DatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> CreateAsync(Group group)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();

            return await db
                .Query(TableName)
                .InsertGetIdAsync<int>(
                    new
                    {
                        prefix = group.Prefix,
                        number = group.Number,
                        is_active = true
                    }
                );
        }

        public async Task DeleteAsync(int id)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();

            await db
                .Query(TableName)
                .Where("id", id)
                .Where("is_active", true)
                .UpdateAsync(
                    new
                    {
                        is_active = false,
                        updated_at = DateTime.Now
                    }
                );
        }

        public async Task<IReadOnlyList<Group>> GetAllAsync()
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();

            IEnumerable<Group> groups =
                await db
                    .Query(TableName)
                    .Where("is_active", true)
                    .Select(
                        "id",
                        "prefix",
                        "number"
                    )
                    .OrderBy("prefix")
                    .OrderBy("number")
                    .GetAsync<Group>();

            return groups.ToList();
        }

        public async Task<Group?> GetByIdAsync(int id)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();

            IEnumerable<Group> groups =
                await db
                    .Query(TableName)
                    .Where("id", id)
                    .Where("is_active", true)
                    .Limit(1)
                    .GetAsync<Group>();

            return groups.FirstOrDefault();
        }

        public async Task<PagedResult<Group>> GetPagedAsync(
            string? search,
            int page,
            int pageSize)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();

            Query query =
                db.Query(TableName)
                    .Where("is_active", true);

            if (!string.IsNullOrWhiteSpace(search))
            {
                string value = search.Trim();

                query.Where(q =>
                {
                    q.WhereContains("prefix", value);

                    if (byte.TryParse(value, out byte number))
                    {
                        q.OrWhere("number", number);
                    }

                    string[] parts = value.Split('-');

                    if (parts.Length == 2 &&
                        byte.TryParse(parts[1], out byte groupNumber))
                    {
                        string prefix = parts[0].Trim();

                        q.OrWhere(inner =>
                        {
                            inner
                                .Where("prefix", prefix)
                                .Where("number", groupNumber);

                            return inner;
                        });
                    }

                    return q;
                });
            }

            IEnumerable<int> countResult =
                await query
                    .Clone()
                    .AsCount()
                    .GetAsync<int>();

            int totalCount =
                countResult.FirstOrDefault();

            IEnumerable<Group> groups =
                await query
                    .Clone()
                    .Select(
                        "id",
                        "prefix",
                        "number",
                        "created_at",
                        "updated_at"
                    )
                    .OrderBy("prefix")
                    .OrderBy("number")
                    .ForPage(page, pageSize)
                    .GetAsync<Group>();

            return new PagedResult<Group>
            {
                Items = groups.ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task UpdateAsync(Group group)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();

            await db
                .Query(TableName)
                .Where("id", group.Id)
                .Where("is_active", true)
                .UpdateAsync(
                    new
                    {
                        prefix = group.Prefix,
                        number = group.Number,
                        updated_at = DateTime.Now
                    }
                );
        }
    }
}
