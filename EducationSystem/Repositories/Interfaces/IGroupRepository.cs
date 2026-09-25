using EducationSystem.DTOs.Common;
using EducationSystem.Models;

namespace EducationSystem.Repositories.Interfaces
{
    public interface IGroupRepository
    {
        Task<PagedResult<Group>> GetPagedAsync(
            string? search,
            int page,
            int pageSize
        );
        Task<Group?> GetByIdAsync(int id);
        Task<IReadOnlyList<Group>> GetAllAsync();
        Task<int> CreateAsync(Group group);
        Task UpdateAsync(Group group);
        Task DeleteAsync(int id);
    }
}
