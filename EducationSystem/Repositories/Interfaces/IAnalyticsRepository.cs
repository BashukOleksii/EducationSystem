using EducationSystem.DTOs.Analytics;

namespace EducationSystem.Repositories.Interfaces
{
    public interface IAnalyticsRepository
    {
        // Викладачі, їх предмети та статистика по кожному предмету
        Task<IReadOnlyList<TeacherSubjectStatisticsItem>>
            GetTeacherSubjectStatisticsAsync(
                int? teacherId = null,
                int? subjectId = null
            );


        // Загальне навантаження викладачів
        Task<IReadOnlyList<TeacherWorkloadItem>>
            GetTeacherWorkloadAsync();


        // Детальний список занять конкретного викладача
        Task<IReadOnlyList<LessonDetailsItem>>
            GetTeacherLessonsAsync(
                int teacherId
            );


        // Загальна статистика предметів
        Task<IReadOnlyList<SubjectStatisticsItem>>
            GetSubjectStatisticsAsync();


        // Детальний список занять конкретного предмета
        Task<IReadOnlyList<LessonDetailsItem>>
            GetSubjectLessonsAsync(
                int subjectId
            );


        // Статистика студентів за предметами
        Task<IReadOnlyList<StudentSubjectStatisticsItem>>
            GetStudentSubjectStatisticsAsync(
                int? studentId = null,
                int? subjectId = null
            );


        // Деталі студента з конкретного предмета:
        // заняття, оцінки, відвідування
        Task<IReadOnlyList<StudentLessonResultItem>>
            GetStudentSubjectDetailsAsync(
                int studentId,
                int subjectId
            );


        // Загальна статистика всіх студентів
        Task<IReadOnlyList<StudentOverallStatisticsItem>>
            GetStudentOverallStatisticsAsync();


        // Студенти групи ризику
        Task<IReadOnlyList<StudentOverallStatisticsItem>>
            GetAtRiskStudentsAsync(
                decimal maxAverageGrade,
                int minMissingCount
            );


        // Найкращі студенти з конкретного предмета
        Task<IReadOnlyList<StudentSubjectStatisticsItem>>
            GetTopStudentsBySubjectAsync(
                int subjectId,
                int count
            );


        // Статистика груп за предметами
        Task<IReadOnlyList<GroupSubjectStatisticsItem>>
            GetGroupSubjectStatisticsAsync(
                int? groupId = null,
                int? subjectId = null
            );


        // Викладачі, які мають предмети,
        // але ще не провели жодного заняття
        Task<IReadOnlyList<TeacherWorkloadItem>>
            GetTeachersWithoutLessonsAsync();
    }
}