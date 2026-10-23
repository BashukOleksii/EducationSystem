using Dapper;
using EducationSystem.Data;
using EducationSystem.DTOs.Common;
using EducationSystem.DTOs.Journal;
using EducationSystem.Models.Enums;
using EducationSystem.Repositories.Interfaces;
using SqlKata;
using SqlKata.Execution;
using System.Data;

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
                int page,
                int pageSize)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();

            const string sql = """
            SELECT COUNT(*)
            FROM dbo.fn_GetLessonJournal(@LessonId)
            WHERE group_id = @GroupId;

            SELECT
                student_id AS StudentId,
                first_name AS FirstName,
                last_name AS LastName,
                email AS Email,
                grade AS Grade,
                attendance_status_value AS AttendanceStatusValue
            FROM dbo.fn_GetLessonJournal(@LessonId)
            WHERE group_id = @GroupId
            ORDER BY
                last_name,
                first_name,
                student_id
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY;
            """;

            using var reader = await db.Connection.QueryMultipleAsync(
                sql,
                new
                {
                    LessonId = lessonId,
                    GroupId = groupId,
                    Offset = (page - 1) * pageSize,
                    PageSize = pageSize
                }
            );

            int totalCount = await reader.ReadSingleAsync<int>();

            List<JournalStudentItem> items =
                (await reader.ReadAsync<JournalStudentItem>())
                .ToList();

            foreach (JournalStudentItem item in items)
            {
                item.IsMissing =
                    item.AttendanceStatusValue == "missing";
            }

            return new PagedResult<JournalStudentItem>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
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

            string? status = attendanceStatus switch
            {
                AttendanceStatus.Present => "present",
                AttendanceStatus.Missing => "missing",
                _ => null
            };

            await db.Connection.ExecuteAsync(
                "dbo.usp_SaveJournalEntry",
                new
                {
                    LessonId = lessonId,
                    StudentId = studentId,
                    Grade = grade,
                    AttendanceStatus = status
                },
                commandType: CommandType.StoredProcedure
            );
        }


        public async Task SavePageAsync(
            int lessonId,
            IReadOnlyList<JournalStudentItem> items)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();

            DataTable table = new();

            table.Columns.Add("StudentId", typeof(int));
            table.Columns.Add("Grade", typeof(byte));
            table.Columns.Add("AttendanceStatus", typeof(string));

            foreach (JournalStudentItem item in items)
            {
                object grade = item.Grade.HasValue
                    ? checked((byte)item.Grade.Value)
                    : DBNull.Value;

                string status = item.IsMissing
                    ? "missing"
                    : "present";

                table.Rows.Add(
                    item.StudentId,
                    grade,
                    status
                );
            }

            await db.Connection.ExecuteAsync(
                "dbo.usp_SaveJournalPage",
                new
                {
                    LessonId = lessonId,
                    Entries = table.AsTableValuedParameter(
                        "dbo.JournalEntryTableType"
                    )
                },
                commandType: CommandType.StoredProcedure
            );
        }


    }
}