using EducationSystem.DTOs.Common;
using EducationSystem.DTOs.Journal;
using EducationSystem.Models;
using EducationSystem.Models.Enums;
using EducationSystem.Repositories.Interfaces;
using System.Globalization;

namespace EducationSystem.Services
{
    public sealed class JournalService
    {
        private readonly IJournalRepository _journalRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly IStudentRepository _studentRepository;

        public JournalService(
            IJournalRepository journalRepository,
            ILessonRepository lessonRepository,
            IStudentRepository studentRepository)
        {
            _journalRepository = journalRepository;
            _lessonRepository = lessonRepository;
            _studentRepository = studentRepository;
        }

        public async Task<PagedResult<JournalStudentItem>> GetPagedAsync(
            int lessonId,
            int page,
            int pageSize)
        {
            if (page < 1)
                page = 1;

            if (pageSize < 1)
                pageSize = 10;

            Lesson? lesson = await _lessonRepository.GetByIdAsync(lessonId);

            if (lesson is null)
                throw new ArgumentException("Заняття не знайдено.");

            return await _journalRepository.GetPagedAsync(
                lessonId, lesson.GroupId, page, pageSize);
        }

        public async Task SaveEntryAsync(int lessonId, JournalStudentItem item)
        {
            ArgumentNullException.ThrowIfNull(item);
            byte? grade = ValidateAndConvertGrade(item);

            Lesson? lesson = await _lessonRepository.GetByIdAsync(lessonId);

            if (lesson is null)
                throw new ArgumentException("Заняття не знайдено.");

            AttendanceStatus status = item.IsMissing
                ? AttendanceStatus.Missing
                : AttendanceStatus.Present;

            await _journalRepository.SaveEntryAsync(
                lessonId, item.StudentId, grade, status);
        }

        public async Task SavePageAsync(
            int lessonId,
            IReadOnlyList<JournalStudentItem> items)
        {
            ArgumentNullException.ThrowIfNull(items);

            foreach (JournalStudentItem item in items)
                ValidateAndConvertGrade(item);

            await _journalRepository.SavePageAsync(lessonId, items);
        }

        private static byte? ValidateAndConvertGrade(JournalStudentItem item)
        {
            string input = item.GradeText;

            if (!string.IsNullOrWhiteSpace(input))
            {
                if (!int.TryParse(input, NumberStyles.None,
                        CultureInfo.InvariantCulture, out int parsed)
                    || parsed < 1 || parsed > 12
                    || item.Grade != parsed)
                {
                    throw new ArgumentException(
                        $"Некоректна оцінка студента {item.FullName}. Введи число від 1 до 12 або залиш поле порожнім.");
                }
            }

            if (item.IsMissing && item.Grade.HasValue)
                throw new ArgumentException(
                    $"Студент {item.FullName} не може одночасно мати оцінку та НБ.");

            if (item.Grade.HasValue && (item.Grade.Value < 1 || item.Grade.Value > 12))
                throw new ArgumentException("Оцінка повинна бути від 1 до 12.");

            return item.Grade.HasValue ? (byte)item.Grade.Value : null;
        }
    }
}
