using EducationSystem.DTOs.Common;
using EducationSystem.DTOs.Lessons;
using EducationSystem.Models;

namespace EducationSystem.Repositories.Interfaces
{
    public interface ILessonRepository
    {
        Task<PagedResult<LessonListItem>> GetPagedAsync(
            string? search,
            int? teacherId,
            int? subjectId,
            int? groupId,
            DateTime? fromDate,
            DateTime? toDate,
            int page,
            int pageSize
        );

        Task<IReadOnlyList<LessonListItem>> GetAllAsync();
        Task<Lesson?> GetByIdAsync(int id);

        Task<int> CreateAsync(Lesson lesson);

        Task UpdateAsync(Lesson lesson);

        Task DeleteAsync(int id);
    }
}