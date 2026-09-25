using EducationSystem.Data;
using EducationSystem.DTOs.Common;
using EducationSystem.DTOs.TeacherSubjects;
using EducationSystem.Models;
using EducationSystem.Repositories.Interfaces;
using SqlKata;
using SqlKata.Execution;

namespace EducationSystem.Repositories
{
    public sealed class TeacherSubjectRepository
        : ITeacherSubjectRepository
    {
        private readonly DatabaseConnectionFactory _connectionFactory;

        private const string TableName =
            "teachers_subjects";


        public TeacherSubjectRepository(
            DatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory =
                connectionFactory;
        }


        public async Task<PagedResult<TeacherSubjectListItem>>
            GetPagedAsync(
                string? search,
                int? teacherId,
                int? subjectId,
                int page,
                int pageSize)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();


            Query query =
                db.Query(TableName)
                    .Join(
                        "teachers",
                        "teachers.id",
                        "teachers_subjects.teacher_id"
                    )
                    .Join(
                        "subjects",
                        "subjects.id",
                        "teachers_subjects.subject_id"
                    );

            if (!string.IsNullOrWhiteSpace(search))
            {
                string[] searchParts =
                    search
                        .Trim()
                        .Split(
                            ' ',
                            StringSplitOptions.RemoveEmptyEntries
                        );


                foreach (string part in searchParts)
                {
                    string value =
                        part;


                    query.Where(q =>
                    {
                        q.WhereContains(
                            "teachers.first_name",
                            value
                        );

                        q.OrWhereContains(
                            "teachers.last_name",
                            value
                        );

                        q.OrWhereContains(
                            "teachers.email",
                            value
                        );

                        q.OrWhereContains(
                            "subjects.name",
                            value
                        );

                        return q;
                    });
                }
            }


            if (teacherId.HasValue)
            {
                query.Where(
                    "teachers_subjects.teacher_id",
                    teacherId.Value
                );
            }

            if (subjectId.HasValue)
            {
                query.Where(
                    "teachers_subjects.subject_id",
                    subjectId.Value
                );
            }


            IEnumerable<int> countResult =
                await query
                    .Clone()
                    .AsCount()
                    .GetAsync<int>();


            int totalCount =
                countResult.FirstOrDefault();


            IEnumerable<TeacherSubjectListItem> items =
                await query
                    .Clone()
                    .Select(
                        "teachers_subjects.id",
                        "teachers_subjects.teacher_id",
                        "teachers_subjects.subject_id",
                        "teachers_subjects.subgroup",

                        "teachers.first_name as teacher_first_name",
                        "teachers.last_name as teacher_last_name",
                        "teachers.email as teacher_email",

                        "subjects.name as subject_name"
                    )
                    .OrderBy("teachers.last_name")
                    .OrderBy("teachers.first_name")
                    .OrderBy("subjects.name")
                    .ForPage(
                        page,
                        pageSize
                    )
                    .GetAsync<TeacherSubjectListItem>();


            return new PagedResult<TeacherSubjectListItem>
            {
                Items =
                    items.ToList(),

                TotalCount =
                    totalCount,

                Page =
                    page,

                PageSize =
                    pageSize
            };
        }


        public async Task<IReadOnlyList<TeacherSubjectListItem>> GetAllAsync()
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();

            IEnumerable<TeacherSubjectListItem> items =
                await db
                    .Query(TableName)
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
                    .Select(
                        "teachers_subjects.id",
                        "teachers_subjects.teacher_id",
                        "teachers_subjects.subject_id",
                        "teachers_subjects.subgroup",

                        "teachers.first_name as teacher_first_name",
                        "teachers.last_name as teacher_last_name",
                        "teachers.email as teacher_email",

                        "subjects.name as subject_name"
                    )
                    .OrderBy("teachers.last_name")
                    .OrderBy("teachers.first_name")
                    .OrderBy("subjects.name")
                    .GetAsync<TeacherSubjectListItem>();

            return items.ToList();
        }

        public async Task<TeacherSubject?> GetByIdAsync(
            int id)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();


            IEnumerable<TeacherSubject> items =
                await db
                    .Query(TableName)
                    .Where(
                        "id",
                        id
                    )
                    .Select(
                        "id",
                        "teacher_id",
                        "subject_id",
                        "subgroup",
                        "created_at",
                        "updated_at"
                    )
                    .Limit(1)
                    .GetAsync<TeacherSubject>();


            return items.FirstOrDefault();
        }


        public async Task<bool> ExistsAsync(
            int teacherId,
            int subjectId,
            int? excludeId = null)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();


            Query query =
                db.Query(TableName)
                    .Where(
                        "teacher_id",
                        teacherId
                    )
                    .Where(
                        "subject_id",
                        subjectId
                    );


            if (excludeId.HasValue)
            {
                query.Where(
                    "id",
                    "<>",
                    excludeId.Value
                );
            }


            IEnumerable<int> ids =
                await query
                    .Select("id")
                    .Limit(1)
                    .GetAsync<int>();


            return ids.Any();
        }


        public async Task<int> CreateAsync(
            TeacherSubject teacherSubject)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();


            int id =
                await db
                    .Query(TableName)
                    .InsertGetIdAsync<int>(
                        new
                        {
                            teacher_id =
                                teacherSubject.TeacherId,

                            subject_id =
                                teacherSubject.SubjectId,

                            subgroup =
                                teacherSubject.Subgroup
                        }
                    );


            return id;
        }


        public async Task UpdateAsync(
            TeacherSubject teacherSubject)
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();


            await db
                .Query(TableName)
                .Where(
                    "id",
                    teacherSubject.Id
                )
                .UpdateAsync(
                    new
                    {
                        teacher_id =
                            teacherSubject.TeacherId,

                        subject_id =
                            teacherSubject.SubjectId,

                        subgroup =
                            teacherSubject.Subgroup,

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