using EducationSystem.DTOs.Common;
using EducationSystem.DTOs.Lessons;
using EducationSystem.Models;
using EducationSystem.Repositories.Interfaces;

namespace EducationSystem.Services
{
    public sealed class LessonService
    {
        private readonly ILessonRepository _lessonRepository;

        private readonly ITeacherSubjectRepository
            _teacherSubjectRepository;

        private readonly IGroupRepository
            _groupRepository;


        public LessonService(
            ILessonRepository lessonRepository,
            ITeacherSubjectRepository teacherSubjectRepository,
            IGroupRepository groupRepository)
        {
            _lessonRepository =
                lessonRepository;

            _teacherSubjectRepository =
                teacherSubjectRepository;

            _groupRepository =
                groupRepository;
        }


        public Task<PagedResult<LessonListItem>> GetPagedAsync(
            string? search,
            int? teacherId,
            int? subjectId,
            int? groupId,
            DateTime? fromDate,
            DateTime? toDate,
            int page,
            int pageSize)
        {
            if (page < 1)
            {
                page = 1;
            }


            if (pageSize < 1)
            {
                pageSize = 10;
            }


            if (fromDate.HasValue &&
                toDate.HasValue &&
                fromDate.Value.Date > toDate.Value.Date)
            {
                throw new ArgumentException(
                    "Початкова дата не може бути пізнішою за кінцеву."
                );
            }


            return _lessonRepository.GetPagedAsync(
                search,
                teacherId,
                subjectId,
                groupId,
                fromDate,
                toDate,
                page,
                pageSize
            );
        }


        public Task<Lesson?> GetByIdAsync(
            int id)
        {
            return _lessonRepository
                .GetByIdAsync(id);
        }


        public async Task<int> CreateAsync(
            string? title,
            int teacherSubjectId,
            int groupId,
            DateTime date)
        {
            await ValidateReferencesAsync(
                teacherSubjectId,
                groupId
            );


            Lesson lesson =
                new Lesson
                {
                    Title =
                        NormalizeTitle(title),

                    TeacherSubjectId =
                        teacherSubjectId,

                    GroupId =
                        groupId,

                    Date =
                        date.Date
                };


            return await _lessonRepository
                .CreateAsync(lesson);
        }


        public async Task UpdateAsync(
            int id,
            string? title,
            int teacherSubjectId,
            int groupId,
            DateTime date)
        {
            await ValidateReferencesAsync(
                teacherSubjectId,
                groupId
            );


            Lesson lesson =
                new Lesson
                {
                    Id =
                        id,

                    Title =
                        NormalizeTitle(title),

                    TeacherSubjectId =
                        teacherSubjectId,

                    GroupId =
                        groupId,

                    Date =
                        date.Date
                };


            await _lessonRepository
                .UpdateAsync(lesson);
        }


        public Task DeleteAsync(
            int id)
        {
            return _lessonRepository
                .DeleteAsync(id);
        }


        private async Task ValidateReferencesAsync(
            int teacherSubjectId,
            int groupId)
        {
            TeacherSubject? teacherSubject =
                await _teacherSubjectRepository
                    .GetByIdAsync(
                        teacherSubjectId
                    );


            if (teacherSubject is null)
            {
                throw new ArgumentException(
                    "Обрану зв'язку викладача і предмета не знайдено."
                );
            }


            Group? group =
                await _groupRepository
                    .GetByIdAsync(
                        groupId
                    );


            if (group is null)
            {
                throw new ArgumentException(
                    "Обрану групу не знайдено."
                );
            }
        }


        private static string? NormalizeTitle(
            string? title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return null;
            }


            return title.Trim();
        }
    }
}