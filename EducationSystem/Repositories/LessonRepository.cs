using EducationSystem.Data;
using EducationSystem.DTOs.Common;
using EducationSystem.DTOs.Lessons;
using EducationSystem.Models;
using EducationSystem.Repositories.Interfaces;
using SqlKata;
using SqlKata.Execution;

namespace EducationSystem.Repositories
{
    public sealed class LessonRepository : ILessonRepository
    {
        private readonly DatabaseConnectionFactory _connectionFactory;

        private const string TableName = "lessons";


        public LessonRepository(
            DatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory =
                connectionFactory;
        }


        public async Task<PagedResult<LessonListItem>> GetPagedAsync(
            string? search,
            int? teacherId,
            int? subjectId,
            int? groupId,
            DateTime? fromDate,
            DateTime? toDate,
            int page,
            int pageSize)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();


            Query query =
                db.Query(TableName)

                    .Join(
                        "teachers_subjects",
                        "teachers_subjects.id",
                        "lessons.teacher_id"
                    )

                    .Join(
                        "teachers",
                        "teachers.id",
                        "teachers_subjects.teacher_id"
                    )

                    .Join(
                        "subjects",
                        "subjects.id",
                        "teachers_subjects.subject_id"
                    )

                    .Join(
                        "groups",
                        "groups.id",
                        "lessons.group_id"
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
                    string value = part;


                    query.WhereContains(
                            "lessons.title",
                            value
                        );
                }
            }


            if (teacherId.HasValue)
            {
                query.Where(
                    "teachers.id",
                    teacherId.Value
                );
            }


            if (subjectId.HasValue)
            {
                query.Where(
                    "subjects.id",
                    subjectId.Value
                );
            }


            if (groupId.HasValue)
            {
                query.Where(
                    "groups.id",
                    groupId.Value
                );
            }


            if (fromDate.HasValue)
            {
                query.Where(
                    "lessons.date",
                    ">=",
                    fromDate.Value.Date
                );
            }


            if (toDate.HasValue)
            {
                query.Where(
                    "lessons.date",
                    "<=",
                    toDate.Value.Date
                );
            }


            IEnumerable<int> countResult =
                await query
                    .Clone()
                    .AsCount()
                    .GetAsync<int>();


            int totalCount =
                countResult.FirstOrDefault();


            IEnumerable<LessonListItem> lessons =
                await query
                    .Clone()
                    .Select(
                        "lessons.id",
                        "lessons.title",
                        "lessons.date",

                        "lessons.teacher_id as teacher_subject_id",

                        "lessons.group_id",

                        "teachers.id as teacher_id",
                        "subjects.id as subject_id",

                        "teachers.first_name as teacher_first_name",
                        "teachers.last_name as teacher_last_name",

                        "subjects.name as subject_name",

                        "teachers_subjects.subgroup",

                        "groups.prefix as group_prefix",
                        "groups.number as group_number"
                    )
                    .OrderByDesc(
                        "lessons.date"
                    )
                    .OrderByDesc(
                        "lessons.id"
                    )
                    .ForPage(
                        page,
                        pageSize
                    )
                    .GetAsync<LessonListItem>();


            return new PagedResult<LessonListItem>
            {
                Items =
                    lessons.ToList(),

                TotalCount =
                    totalCount,

                Page =
                    page,

                PageSize =
                    pageSize
            };
        }

        public async Task<IReadOnlyList<LessonListItem>> GetAllAsync()
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();


            IEnumerable<LessonListItem> lessons =
                await db
                    .Query(TableName)

                    .Join(
                        "teachers_subjects",
                        "teachers_subjects.id",
                        "lessons.teacher_id"
                    )

                    .Join(
                        "teachers",
                        "teachers.id",
                        "teachers_subjects.teacher_id"
                    )

                    .Join(
                        "subjects",
                        "subjects.id",
                        "teachers_subjects.subject_id"
                    )

                    .Join(
                        "groups",
                        "groups.id",
                        "lessons.group_id"
                    )

                    .Select(
                        "lessons.id",
                        "lessons.title",
                        "lessons.date",

                        "lessons.teacher_id as teacher_subject_id",
                        "lessons.group_id",

                        "teachers.id as teacher_id",
                        "subjects.id as subject_id",

                        "teachers.first_name as teacher_first_name",
                        "teachers.last_name as teacher_last_name",

                        "subjects.name as subject_name",

                        "teachers_subjects.subgroup",

                        "groups.prefix as group_prefix",
                        "groups.number as group_number"
                    )

                    .OrderByDesc("lessons.date")
                    .OrderByDesc("lessons.id")

                    .GetAsync<LessonListItem>();


            return lessons.ToList();
        }
        public async Task<Lesson?> GetByIdAsync(
            int id)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();


            IEnumerable<Lesson> lessons =
                await db
                    .Query(TableName)
                    .Where(
                        "id",
                        id
                    )
                    .Select(
                        "id",
                        "title",

                        "teacher_id as teacher_subject_id",

                        "group_id",
                        "date",
                        "created_at",
                        "updated_at"
                    )
                    .Limit(1)
                    .GetAsync<Lesson>();


            return lessons.FirstOrDefault();
        }


        public async Task<int> CreateAsync(
            Lesson lesson)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();


            int id =
                await db
                    .Query(TableName)
                    .InsertGetIdAsync<int>(
                        new
                        {
                            title =
                                lesson.Title,
                            teacher_id =
                                lesson.TeacherSubjectId,

                            group_id =
                                lesson.GroupId,

                            date =
                                lesson.Date.Date
                        }
                    );


            return id;
        }


        public async Task UpdateAsync(
            Lesson lesson)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();


            await db
                .Query(TableName)
                .Where(
                    "id",
                    lesson.Id
                )
                .UpdateAsync(
                    new
                    {
                        title =
                            lesson.Title,

                        teacher_id =
                            lesson.TeacherSubjectId,

                        group_id =
                            lesson.GroupId,

                        date =
                            lesson.Date.Date,

                        updated_at =
                            DateTime.Now
                    }
                );
        }


        public async Task DeleteAsync(
            int id)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();


            await db
                .Query(TableName)
                .Where(
                    "id",
                    id
                )
                .DeleteAsync();
        }
    }
}