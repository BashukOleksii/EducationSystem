using EducationSystem.DTOs.Common;
using EducationSystem.DTOs.Journal;
using EducationSystem.Models;
using EducationSystem.Models.Enums;
using EducationSystem.Repositories.Interfaces;

namespace EducationSystem.Services
{
    public sealed class JournalService
    {
        private readonly IJournalRepository
            _journalRepository;

        private readonly ILessonRepository
            _lessonRepository;

        private readonly IStudentRepository
            _studentRepository;


        public JournalService(
            IJournalRepository journalRepository,
            ILessonRepository lessonRepository,
            IStudentRepository studentRepository)
        {
            _journalRepository =
                journalRepository;

            _lessonRepository =
                lessonRepository;

            _studentRepository =
                studentRepository;
        }


        public async Task<PagedResult<JournalStudentItem>>
            GetPagedAsync(
                int lessonId,
                string? search,
                bool onlyMissing,
                bool onlyWithoutGrade,
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


            Lesson? lesson =
                await _lessonRepository
                    .GetByIdAsync(
                        lessonId
                    );


            if (lesson is null)
            {
                throw new ArgumentException(
                    "Заняття не знайдено."
                );
            }


            return await _journalRepository
                .GetPagedAsync(
                    lessonId,
                    lesson.GroupId,
                    search,
                    onlyMissing,
                    onlyWithoutGrade,
                    page,
                    pageSize
                );
        }


        public async Task SaveEntryAsync(
            int lessonId,
            JournalStudentItem item)
        {
            Lesson? lesson =
                await _lessonRepository
                    .GetByIdAsync(
                        lessonId
                    );


            if (lesson is null)
            {
                throw new ArgumentException(
                    "Заняття не знайдено."
                );
            }


            Student? student =
                await _studentRepository
                    .GetByIdAsync(
                        item.StudentId
                    );


            if (student is null)
            {
                throw new ArgumentException(
                    "Студента не знайдено."
                );
            }

            if (student.GroupId != lesson.GroupId)
            {
                throw new ArgumentException(
                    "Студент не належить до групи цього заняття."
                );
            }


            byte? grade =
                ValidateAndConvertGrade(
                    item.Grade
                );


            AttendanceStatus? attendanceStatus =
                item.IsMissing switch
                {
                    true =>
                        AttendanceStatus.Missing,

                    false =>
                        AttendanceStatus.Present,

                    null =>
                        null
                };


            await _journalRepository
                .SaveEntryAsync(
                    lessonId,
                    item.StudentId,
                    grade,
                    attendanceStatus
                );
        }


        private static byte? ValidateAndConvertGrade(
            int? grade)
        {
            if (!grade.HasValue)
            {
                return null;
            }

            if (grade.Value is < 0 or > 255)
            {
                throw new ArgumentException(
                    "Оцінка повинна бути від 0 до 255."
                );
            }


            return (byte)grade.Value;
        }
    }
}