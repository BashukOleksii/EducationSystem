using EducationSystem.DTOs.Common;
using EducationSystem.DTOs.TeacherSubjects;
using EducationSystem.Models;

namespace EducationSystem.Repositories.Interfaces
{
    public interface ITeacherSubjectRepository
    {
        Task<PagedResult<TeacherSubjectListItem>> GetPagedAsync(
            string? search,
            int? teacherId,
            int? subjectId,
            int page,
            int pageSize
        );

        Task<TeacherSubject?> GetByIdAsync(int id);

        Task<IReadOnlyList<TeacherSubjectListItem>> GetAllAsync();
        Task<bool> ExistsAsync(
            int teacherId,
            int subjectId,
            int? excludeId = null
        );

        Task<int> CreateAsync(
            TeacherSubject teacherSubject
        );

        Task UpdateAsync(
            TeacherSubject teacherSubject
        );

        Task DeleteAsync(int id);
    }
}