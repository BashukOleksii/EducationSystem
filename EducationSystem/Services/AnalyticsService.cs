using EducationSystem.DTOs.Analytics;
using EducationSystem.Repositories.Interfaces;

namespace EducationSystem.Services
{
    public sealed class AnalyticsService
    {
        private readonly IAnalyticsRepository _analyticsRepository;


        public AnalyticsService(
            IAnalyticsRepository analyticsRepository)
        {
            _analyticsRepository =
                analyticsRepository;
        }

        public Task<IReadOnlyList<TeacherSubjectStatisticsItem>>
            GetTeacherSubjectStatisticsAsync(
                int? teacherId = null,
                int? subjectId = null)
        {
            ValidateOptionalId(
                teacherId,
                "Ідентифікатор викладача"
            );

            ValidateOptionalId(
                subjectId,
                "Ідентифікатор предмета"
            );


            return _analyticsRepository
                .GetTeacherSubjectStatisticsAsync(
                    teacherId,
                    subjectId
                );
        }



        public Task<IReadOnlyList<TeacherWorkloadItem>>
            GetTeacherWorkloadAsync()
        {
            return _analyticsRepository
                .GetTeacherWorkloadAsync();
        }



        public Task<IReadOnlyList<LessonDetailsItem>>
            GetTeacherLessonsAsync(
                int teacherId)
        {
            ValidateId(
                teacherId,
                "Ідентифікатор викладача"
            );


            return _analyticsRepository
                .GetTeacherLessonsAsync(
                    teacherId
                );
        }


        public Task<IReadOnlyList<SubjectStatisticsItem>>
            GetSubjectStatisticsAsync()
        {
            return _analyticsRepository
                .GetSubjectStatisticsAsync();
        }



        public Task<IReadOnlyList<LessonDetailsItem>>
            GetSubjectLessonsAsync(
                int subjectId)
        {
            ValidateId(
                subjectId,
                "Ідентифікатор предмета"
            );


            return _analyticsRepository
                .GetSubjectLessonsAsync(
                    subjectId
                );
        }



        public Task<IReadOnlyList<StudentSubjectStatisticsItem>>
            GetStudentSubjectStatisticsAsync(
                int? studentId = null,
                int? subjectId = null)
        {
            ValidateOptionalId(
                studentId,
                "Ідентифікатор студента"
            );

            ValidateOptionalId(
                subjectId,
                "Ідентифікатор предмета"
            );


            return _analyticsRepository
                .GetStudentSubjectStatisticsAsync(
                    studentId,
                    subjectId
                );
        }


        public Task<IReadOnlyList<StudentLessonResultItem>>
            GetStudentSubjectDetailsAsync(
                int studentId,
                int subjectId)
        {
            ValidateId(
                studentId,
                "Ідентифікатор студента"
            );

            ValidateId(
                subjectId,
                "Ідентифікатор предмета"
            );


            return _analyticsRepository
                .GetStudentSubjectDetailsAsync(
                    studentId,
                    subjectId
                );
        }


        public Task<IReadOnlyList<StudentOverallStatisticsItem>>
            GetStudentOverallStatisticsAsync()
        {
            return _analyticsRepository
                .GetStudentOverallStatisticsAsync();
        }



        public Task<IReadOnlyList<StudentOverallStatisticsItem>>
            GetAtRiskStudentsAsync(
                decimal maxAverageGrade,
                int minMissingCount)
        {
            if (maxAverageGrade < 1 ||
                maxAverageGrade > 12)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxAverageGrade),
                    "Граничний середній бал має бути від 1 до 12."
                );
            }


            if (minMissingCount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(minMissingCount),
                    "Кількість пропусків не може бути від'ємною."
                );
            }


            return _analyticsRepository
                .GetAtRiskStudentsAsync(
                    maxAverageGrade,
                    minMissingCount
                );
        }



        public Task<IReadOnlyList<StudentSubjectStatisticsItem>>
            GetTopStudentsBySubjectAsync(
                int subjectId,
                int count)
        {
            ValidateId(
                subjectId,
                "Ідентифікатор предмета"
            );


            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(count),
                    "Кількість студентів має бути більшою за нуль."
                );
            }


            return _analyticsRepository
                .GetTopStudentsBySubjectAsync(
                    subjectId,
                    count
                );
        }



        public Task<IReadOnlyList<GroupSubjectStatisticsItem>>
            GetGroupSubjectStatisticsAsync(
                int? groupId = null,
                int? subjectId = null)
        {
            ValidateOptionalId(
                groupId,
                "Ідентифікатор групи"
            );

            ValidateOptionalId(
                subjectId,
                "Ідентифікатор предмета"
            );


            return _analyticsRepository
                .GetGroupSubjectStatisticsAsync(
                    groupId,
                    subjectId
                );
        }



        public Task<IReadOnlyList<TeacherWorkloadItem>>
            GetTeachersWithoutLessonsAsync()
        {
            return _analyticsRepository
                .GetTeachersWithoutLessonsAsync();
        }



        private static void ValidateId(
            int id,
            string parameterName)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    $"{parameterName} має бути більшим за нуль."
                );
            }
        }


        private static void ValidateOptionalId(
            int? id,
            string parameterName)
        {
            if (id.HasValue &&
                id.Value <= 0)
            {
                throw new ArgumentException(
                    $"{parameterName} має бути більшим за нуль."
                );
            }
        }
    }
}