using EducationSystem.DTOs.Common;
using EducationSystem.DTOs.Journal;
using EducationSystem.Models.Enums;

namespace EducationSystem.Repositories.Interfaces
{
    public interface IJournalRepository
    {
        Task<PagedResult<JournalStudentItem>> GetPagedAsync(
            int lessonId,
            int groupId,
            string? search,
            bool onlyMissing,
            bool onlyWithoutGrade,
            int page,
            int pageSize
        );

        Task SaveEntryAsync(
            int lessonId,
            int studentId,
            byte? grade,
            AttendanceStatus? attendanceStatus
        );
    }
}