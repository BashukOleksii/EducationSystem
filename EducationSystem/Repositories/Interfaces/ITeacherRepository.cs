using EducationSystem.DTOs.Common;
using EducationSystem.Models;
using EducationSystem.Models.Enums;

namespace EducationSystem.Repositories.Interfaces
{
    public interface ITeacherRepository
    {
        Task<PagedResult<Teacher>> GetPagedAsync(
            string? search,
            TeacherCategory? category,
            int page,
            int pageSize
        );

        Task<Teacher?> GetByIdAsync(int id);

        Task<IReadOnlyList<Teacher>> GetAllAsync();
        Task<bool> ExistsByEmailAsync(
            string email,
            int? excludeId = null
        );

        Task<int> CreateAsync(Teacher teacher);

        Task UpdateAsync(Teacher teacher);

        Task DeleteAsync(int id);
    }
}