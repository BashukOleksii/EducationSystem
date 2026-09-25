using EducationSystem.Data;
using EducationSystem.DTOs.Common;
using EducationSystem.DTOs.Journal;
using EducationSystem.Models.Enums;
using EducationSystem.Repositories.Interfaces;
using SqlKata;
using SqlKata.Execution;

namespace EducationSystem.Repositories
{
    public sealed class JournalRepository
        : IJournalRepository
    {
        private readonly DatabaseConnectionFactory _connectionFactory;


        public JournalRepository(
            DatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory =
                connectionFactory;
        }


        public async Task<PagedResult<JournalStudentItem>>
            GetPagedAsync(
                int lessonId,
                int groupId,
                string? search,
                bool onlyMissing,
                bool onlyWithoutGrade,
                int page,
                int pageSize)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();

            Query query =
                db.Query("students")

                    .LeftJoin(
                        "grades",
                        join =>
                            join
                                .On(
                                    "grades.student_id",
                                    "students.id"
                                )
                                .Where(
                                    "grades.assessment_id",
                                    lessonId
                                )
                    )

                    .LeftJoin(
                        "attendance",
                        join =>
                            join
                                .On(
                                    "attendance.student_id",
                                    "students.id"
                                )
                                .Where(
                                    "attendance.lesson_id",
                                    lessonId
                                )
                    )

                    .Where(
                        "students.group_id",
                        groupId
                    );


            if (!string.IsNullOrWhiteSpace(search))
            {
                string[] parts =
                    search
                        .Trim()
                        .Split(
                            ' ',
                            StringSplitOptions.RemoveEmptyEntries
                        );


                foreach (string part in parts)
                {
                    string value =
                        part;


                    query.Where(q =>
                    {
                        q.WhereContains(
                            "students.first_name",
                            value
                        );

                        q.OrWhereContains(
                            "students.last_name",
                            value
                        );

                        q.OrWhereContains(
                            "students.email",
                            value
                        );

                        return q;
                    });
                }
            }


            if (onlyMissing)
            {
                query.Where(
                    "attendance.status",
                    "missing"
                );
            }


            if (onlyWithoutGrade)
            {
                query.WhereNull(
                    "grades.id"
                );
            }


            IEnumerable<int> countResult =
                await query
                    .Clone()
                    .AsCount()
                    .GetAsync<int>();


            int totalCount =
                countResult.FirstOrDefault();


            IEnumerable<JournalStudentItem> result =
                await query
                    .Clone()

                    .Select(
                        "students.id as student_id",
                        "students.first_name",
                        "students.last_name",
                        "students.email",

                        "grades.grade",

                        "attendance.status as attendance_status_value"
                    )

                    .OrderBy(
                        "students.last_name"
                    )

                    .OrderBy(
                        "students.first_name"
                    )

                    .ForPage(
                        page,
                        pageSize
                    )

                    .GetAsync<JournalStudentItem>();


            List<JournalStudentItem> items =
                result.ToList();

            foreach (JournalStudentItem item in items)
            {
                item.IsMissing =
                    item.AttendanceStatusValue switch
                    {
                        "missing" => true,
                        "present" => false,
                        _ => null
                    };
            }


            return new PagedResult<JournalStudentItem>
            {
                Items =
                    items,

                TotalCount =
                    totalCount,

                Page =
                    page,

                PageSize =
                    pageSize
            };
        }


        public async Task SaveEntryAsync(
            int lessonId,
            int studentId,
            byte? grade,
            AttendanceStatus? attendanceStatus)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();


            await SaveGradeAsync(
                db,
                lessonId,
                studentId,
                grade
            );


            await SaveAttendanceAsync(
                db,
                lessonId,
                studentId,
                attendanceStatus
            );
        }


        private static async Task SaveGradeAsync(
            QueryFactory db,
            int lessonId,
            int studentId,
            byte? grade)
        {
            List<int> ids =
                (
                    await db
                        .Query("grades")
                        .Where(
                            "assessment_id",
                            lessonId
                        )
                        .Where(
                            "student_id",
                            studentId
                        )
                        .Select("id")
                        .Limit(1)
                        .GetAsync<int>()
                )
                .ToList();


            bool exists =
                ids.Count > 0;

            if (!grade.HasValue)
            {
                if (exists)
                {
                    await db
                        .Query("grades")
                        .Where(
                            "id",
                            ids[0]
                        )
                        .DeleteAsync();
                }


                return;
            }


            if (exists)
            {
                await db
                    .Query("grades")
                    .Where(
                        "id",
                        ids[0]
                    )
                    .UpdateAsync(
                        new
                        {
                            grade =
                                grade.Value,

                            updated_at =
                                DateTime.Now
                        }
                    );


                return;
            }


            await db
                .Query("grades")
                .InsertAsync(
                    new
                    {
                        assessment_id =
                            lessonId,

                        student_id =
                            studentId,

                        grade =
                            grade.Value
                    }
                );
        }


        private static async Task SaveAttendanceAsync(
            QueryFactory db,
            int lessonId,
            int studentId,
            AttendanceStatus? attendanceStatus)
        {
            List<int> ids =
                (
                    await db
                        .Query("attendance")
                        .Where(
                            "lesson_id",
                            lessonId
                        )
                        .Where(
                            "student_id",
                            studentId
                        )
                        .Select("id")
                        .Limit(1)
                        .GetAsync<int>()
                )
                .ToList();


            bool exists =
                ids.Count > 0;

            if (!attendanceStatus.HasValue)
            {
                if (exists)
                {
                    await db
                        .Query("attendance")
                        .Where(
                            "id",
                            ids[0]
                        )
                        .DeleteAsync();
                }


                return;
            }


            string status =
                attendanceStatus.Value ==
                AttendanceStatus.Missing
                    ? "missing"
                    : "present";

            if (exists)
            {
                await db
                    .Query("attendance")
                    .Where(
                        "id",
                        ids[0]
                    )
                    .UpdateAsync(
                        new
                        {
                            status,

                            updated_at =
                                DateTime.Now
                        }
                    );


                return;
            }


            await db
                .Query("attendance")
                .InsertAsync(
                    new
                    {
                        lesson_id =
                            lessonId,

                        student_id =
                            studentId,

                        status
                    }
                );
        }
    }
}