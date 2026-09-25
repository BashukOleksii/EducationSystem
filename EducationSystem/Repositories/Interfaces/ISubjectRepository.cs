using EducationSystem.DTOs.Common;
using EducationSystem.Models;

namespace EducationSystem.Repositories.Interfaces
{
    public interface ISubjectRepository
    {
        Task<PagedResult<Subject>> GetPagedAsync(
            string? search,
            int page,
            int pageSize
        );

        Task<Subject?> GetByIdAsync(int id);

        Task<bool> ExistsByNameAsync(
            string name,
            int? excludeId = null
        );

        Task<IReadOnlyList<Subject>> GetAllAsync();

        Task<int> CreateAsync(Subject subject);

        Task UpdateAsync(Subject subject);

        Task DeleteAsync(int id);
    }
}