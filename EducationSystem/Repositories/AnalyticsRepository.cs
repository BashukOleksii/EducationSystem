using EducationSystem.Data;
using EducationSystem.DTOs.Analytics;
using EducationSystem.Repositories.Interfaces;
using SqlKata;
using SqlKata.Execution;

namespace EducationSystem.Repositories
{
    public sealed class AnalyticsRepository
        : IAnalyticsRepository
    {
        private readonly DatabaseConnectionFactory _connectionFactory;


        private const string TeacherSubjectStatisticsView =
            "vw_teacher_subject_statistics";

        private const string TeacherWorkloadView =
            "vw_teacher_workload";

        private const string LessonDetailsView =
            "vw_lesson_details";

        private const string SubjectStatisticsView =
            "vw_subject_statistics";

        private const string StudentSubjectStatisticsView =
            "vw_student_subject_statistics";

        private const string StudentLessonResultsView =
            "vw_student_lesson_results";

        private const string StudentOverallStatisticsView =
            "vw_student_overall_statistics";

        private const string GroupSubjectStatisticsView =
            "vw_group_subject_statistics";


        public AnalyticsRepository(
            DatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory =
                connectionFactory;
        }


        public async Task<IReadOnlyList<TeacherSubjectStatisticsItem>>
            GetTeacherSubjectStatisticsAsync(
                int? teacherId = null,
                int? subjectId = null)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();


            Query query =
                db.Query(TeacherSubjectStatisticsView)
                    .Where(
                        "assignment_is_active",
                        true
                    )
                    .Where(
                        "teacher_is_active",
                        true
                    )
                    .Where(
                        "subject_is_active",
                        true
                    );


            if (teacherId.HasValue)
            {
                query.Where(
                    "teacher_id",
                    teacherId.Value
                );
            }


            if (subjectId.HasValue)
            {
                query.Where(
                    "subject_id",
                    subjectId.Value
                );
            }


            IEnumerable<TeacherSubjectStatisticsItem> items =
                await query
                    .OrderBy(
                        "teacher_last_name"
                    )
                    .OrderBy(
                        "teacher_first_name"
                    )
                    .OrderBy(
                        "subject_name"
                    )
                    .GetAsync<TeacherSubjectStatisticsItem>();


            return items.ToList();
        }

        public async Task<IReadOnlyList<TeacherWorkloadItem>>
            GetTeacherWorkloadAsync()
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();


            IEnumerable<TeacherWorkloadItem> items =
                await db
                    .Query(TeacherWorkloadView)
                    .Where(
                        "teacher_is_active",
                        true
                    )
                    .OrderByDesc(
                        "conducted_hours"
                    )
                    .OrderBy(
                        "teacher_last_name"
                    )
                    .OrderBy(
                        "teacher_first_name"
                    )
                    .GetAsync<TeacherWorkloadItem>();


            return items.ToList();
        }



        public async Task<IReadOnlyList<LessonDetailsItem>>
            GetTeacherLessonsAsync(
                int teacherId)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();


            IEnumerable<LessonDetailsItem> items =
                await db
                    .Query(LessonDetailsView)
                    .Where(
                        "teacher_id",
                        teacherId
                    )
                    .OrderByDesc(
                        "lesson_date"
                    )
                    .OrderByDesc(
                        "lesson_id"
                    )
                    .GetAsync<LessonDetailsItem>();


            return items.ToList();
        }



        public async Task<IReadOnlyList<SubjectStatisticsItem>>
            GetSubjectStatisticsAsync()
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();


            IEnumerable<SubjectStatisticsItem> items =
                await db
                    .Query(SubjectStatisticsView)
                    .Where(
                        "subject_is_active",
                        true
                    )
                    .OrderBy(
                        "subject_name"
                    )
                    .GetAsync<SubjectStatisticsItem>();


            return items.ToList();
        }



        public async Task<IReadOnlyList<LessonDetailsItem>>
            GetSubjectLessonsAsync(
                int subjectId)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();


            IEnumerable<LessonDetailsItem> items =
                await db
                    .Query(LessonDetailsView)
                    .Where(
                        "subject_id",
                        subjectId
                    )
                    .OrderByDesc(
                        "lesson_date"
                    )
                    .OrderByDesc(
                        "lesson_id"
                    )
                    .GetAsync<LessonDetailsItem>();


            return items.ToList();
        }



        public async Task<IReadOnlyList<StudentSubjectStatisticsItem>>
            GetStudentSubjectStatisticsAsync(
                int? studentId = null,
                int? subjectId = null)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();


            Query query =
                db.Query(StudentSubjectStatisticsView);


            if (studentId.HasValue)
            {
                query.Where(
                    "student_id",
                    studentId.Value
                );
            }


            if (subjectId.HasValue)
            {
                query.Where(
                    "subject_id",
                    subjectId.Value
                );
            }


            IEnumerable<StudentSubjectStatisticsItem> items =
                await query
                    .OrderBy(
                        "student_last_name"
                    )
                    .OrderBy(
                        "student_first_name"
                    )
                    .OrderBy(
                        "subject_name"
                    )
                    .GetAsync<StudentSubjectStatisticsItem>();


            return items.ToList();
        }


        public async Task<IReadOnlyList<StudentLessonResultItem>>
            GetStudentSubjectDetailsAsync(
                int studentId,
                int subjectId)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();


            IEnumerable<StudentLessonResultItem> items =
                await db
                    .Query(StudentLessonResultsView)
                    .Where(
                        "student_id",
                        studentId
                    )
                    .Where(
                        "subject_id",
                        subjectId
                    )
                    .OrderByDesc(
                        "lesson_date"
                    )
                    .OrderByDesc(
                        "lesson_id"
                    )
                    .GetAsync<StudentLessonResultItem>();


            return items.ToList();
        }



        public async Task<IReadOnlyList<StudentOverallStatisticsItem>>
            GetStudentOverallStatisticsAsync()
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();


            IEnumerable<StudentOverallStatisticsItem> items =
                await db
                    .Query(StudentOverallStatisticsView)
                    .OrderBy(
                        "student_last_name"
                    )
                    .OrderBy(
                        "student_first_name"
                    )
                    .GetAsync<StudentOverallStatisticsItem>();


            return items.ToList();
        }

        public async Task<IReadOnlyList<StudentOverallStatisticsItem>>
            GetAtRiskStudentsAsync(
                decimal maxAverageGrade,
                int minMissingCount)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();


            Query query =
                db.Query(StudentOverallStatisticsView);


            query.Where(q =>
            {
                q.Where(
                    "average_grade",
                    "<",
                    maxAverageGrade
                );

                q.OrWhereNull(
                    "average_grade"
                );

                q.OrWhere(
                    "missing_count",
                    ">=",
                    minMissingCount
                );

                return q;
            });


            IEnumerable<StudentOverallStatisticsItem> items =
                await query
                    .OrderByDesc(
                        "missing_count"
                    )
                    .OrderBy(
                        "average_grade"
                    )
                    .OrderBy(
                        "student_last_name"
                    )
                    .GetAsync<StudentOverallStatisticsItem>();


            return items.ToList();
        }



        public async Task<IReadOnlyList<StudentSubjectStatisticsItem>>
            GetTopStudentsBySubjectAsync(
                int subjectId,
                int count)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();


            IEnumerable<StudentSubjectStatisticsItem> items =
                await db
                    .Query(StudentSubjectStatisticsView)

                    .Where(
                        "subject_id",
                        subjectId
                    )

                    .WhereNotNull(
                        "average_grade"
                    )

                    .OrderByDesc(
                        "average_grade"
                    )

                    .OrderBy(
                        "missing_count"
                    )

                    .OrderBy(
                        "student_last_name"
                    )

                    .Limit(count)

                    .GetAsync<StudentSubjectStatisticsItem>();


            return items.ToList();
        }



        public async Task<IReadOnlyList<GroupSubjectStatisticsItem>>
            GetGroupSubjectStatisticsAsync(
                int? groupId = null,
                int? subjectId = null)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();


            Query query =
                db.Query(GroupSubjectStatisticsView);


            if (groupId.HasValue)
            {
                query.Where(
                    "group_id",
                    groupId.Value
                );
            }


            if (subjectId.HasValue)
            {
                query.Where(
                    "subject_id",
                    subjectId.Value
                );
            }


            IEnumerable<GroupSubjectStatisticsItem> items =
                await query
                    .OrderBy(
                        "group_prefix"
                    )
                    .OrderBy(
                        "group_number"
                    )
                    .OrderBy(
                        "subject_name"
                    )
                    .GetAsync<GroupSubjectStatisticsItem>();


            return items.ToList();
        }


        public async Task<IReadOnlyList<TeacherWorkloadItem>>
            GetTeachersWithoutLessonsAsync()
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();


            IEnumerable<TeacherWorkloadItem> items =
                await db
                    .Query(TeacherWorkloadView)

                    .Where(
                        "teacher_is_active",
                        true
                    )

                    .Where(
                        "active_subject_count",
                        ">",
                        0
                    )

                    .Where(
                        "lesson_count",
                        0
                    )

                    .OrderBy(
                        "teacher_last_name"
                    )

                    .OrderBy(
                        "teacher_first_name"
                    )

                    .GetAsync<TeacherWorkloadItem>();


            return items.ToList();
        }
    }
}