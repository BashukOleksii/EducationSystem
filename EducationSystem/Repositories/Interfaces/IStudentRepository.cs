using EducationSystem.DTOs.Common;
using EducationSystem.DTOs.Students;
using EducationSystem.Models;

namespace EducationSystem.Repositories.Interfaces
{
    public interface IStudentRepository
    {
        Task<PagedResult<StudentListItem>> GetPagedAsync(
            string? search,
            int? groupId,
            int page,
            int pageSize
        );

        Task<Student?> GetByIdAsync(int id);

        Task<bool> ExistsByEmailAsync(
            string email,
            int? excludeId = null
        );

        Task<int> CreateAsync(Student student);

        Task UpdateAsync(Student student);

        Task DeleteAsync(int id);
    }
}