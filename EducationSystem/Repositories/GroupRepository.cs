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
        DatabaseConnectionFactory _connectionFactory;
        const string tableName = "groups";

        public GroupRepository(DatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> CreateAsync(Group group)
        {
            QueryFactory db = _connectionFactory.CreateQueryFactory();

            int id = await
                db.Query(tableName)
                .InsertGetIdAsync<int>(
                    new
                    {
                        prefix = group.Prefix,
                        number = group.Number
                    }
                );

            return id;
        }

        public async Task DeleteAsync(int id)
        {
            QueryFactory db = _connectionFactory.CreateQueryFactory();

            await db
                .Query(tableName)
                .Where("id", id)
                .DeleteAsync();
        }

        public async Task<Group?> GetByIdAsync(int id)
        {
            using QueryFactory db = _connectionFactory.CreateQueryFactory();

            IEnumerable<Group> group = await db
                .Query(tableName)
                .Where("id", id)
                .Limit(1)
                .GetAsync<Group>();

            return group.FirstOrDefault();
        }

        public async Task<PagedResult<Group>> GetPagedAsync(string? search, int page, int pageSize)
        {
            using QueryFactory db = _connectionFactory.CreateQueryFactory();

            Query query= db.Query(tableName);

            if (!string.IsNullOrWhiteSpace(search))
            {
                string value = search.Trim();

                query.Where(q => 
                {
                    q.WhereContains("prefix", value);

                    if(byte.TryParse(value, out byte result))
                        q.OrWhere("number", result);
                    
                    return q;

                });
            }

            IEnumerable<int> countResult = await query.Clone().AsCount().GetAsync<int>();

            int totalCount = countResult.FirstOrDefault();

            IEnumerable<Group> groups = await query
                .Clone()
                .Select(
                    "id",
                    "prefix",
                    "number",
                    "created_at",
                    "updated_at"
                ).OrderBy("prefix")
                .OrderBy("number")
                .ForPage(
                    page,
                    pageSize
                )
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
            QueryFactory db = _connectionFactory.CreateQueryFactory();

            await db
                .Query(tableName)
                .Where("id", group.Id)
                .UpdateAsync(
                new
                {
                    prefix = group.Prefix,
                    number = group.Number,
                    updated_at = DateTime.Now
                });
        }
    }
}
