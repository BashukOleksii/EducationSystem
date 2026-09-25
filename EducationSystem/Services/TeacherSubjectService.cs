using EducationSystem.DTOs.Common;
using EducationSystem.DTOs.TeacherSubjects;
using EducationSystem.Models;
using EducationSystem.Repositories.Interfaces;

namespace EducationSystem.Services
{
    public sealed class TeacherSubjectService
    {
        private readonly ITeacherSubjectRepository
            _teacherSubjectRepository;

        private readonly ITeacherRepository
            _teacherRepository;

        private readonly ISubjectRepository
            _subjectRepository;


        public TeacherSubjectService(
            ITeacherSubjectRepository teacherSubjectRepository,
            ITeacherRepository teacherRepository,
            ISubjectRepository subjectRepository)
        {
            _teacherSubjectRepository =
                teacherSubjectRepository;

            _teacherRepository =
                teacherRepository;

            _subjectRepository =
                subjectRepository;
        }


        public Task<PagedResult<TeacherSubjectListItem>>
            GetPagedAsync(
                string? search,
                int? teacherId,
                int? subjectId,
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


            return _teacherSubjectRepository.GetPagedAsync(
                search,
                teacherId,
                subjectId,
                page,
                pageSize
            );
        }


        public Task<TeacherSubject?> GetByIdAsync(
            int id)
        {
            return _teacherSubjectRepository
                .GetByIdAsync(id);
        }

        public Task<IReadOnlyList<TeacherSubjectListItem>> GetAllAsync()
        {
            return _teacherSubjectRepository.GetAllAsync();
        }

        public async Task<int> CreateAsync(
            int teacherId,
            int subjectId,
            int? subgroup)
        {
            ValidateSubgroup(
                subgroup
            );


            await ValidateReferencesAsync(
                teacherId,
                subjectId
            );


            bool exists =
                await _teacherSubjectRepository
                    .ExistsAsync(
                        teacherId,
                        subjectId
                    );


            if (exists)
            {
                throw new ArgumentException(
                    "Цей предмет уже закріплений за вибраним викладачем."
                );
            }


            TeacherSubject teacherSubject =
                new TeacherSubject
                {
                    TeacherId =
                        teacherId,

                    SubjectId =
                        subjectId,

                    Subgroup =
                        subgroup.HasValue
                            ? (byte)subgroup.Value
                            : null
                };


            return await _teacherSubjectRepository
                .CreateAsync(
                    teacherSubject
                );
        }


        public async Task UpdateAsync(
            int id,
            int teacherId,
            int subjectId,
            int? subgroup)
        {
            ValidateSubgroup(
                subgroup
            );


            await ValidateReferencesAsync(
                teacherId,
                subjectId
            );


            bool exists =
                await _teacherSubjectRepository
                    .ExistsAsync(
                        teacherId,
                        subjectId,
                        id
                    );


            if (exists)
            {
                throw new ArgumentException(
                    "Інший запис із цим викладачем і предметом уже існує."
                );
            }


            TeacherSubject teacherSubject =
                new TeacherSubject
                {
                    Id =
                        id,

                    TeacherId =
                        teacherId,

                    SubjectId =
                        subjectId,

                    Subgroup =
                        subgroup.HasValue
                            ? (byte)subgroup.Value
                            : null
                };


            await _teacherSubjectRepository
                .UpdateAsync(
                    teacherSubject
                );
        }


        public Task DeleteAsync(
            int id)
        {
            return _teacherSubjectRepository
                .DeleteAsync(id);
        }


        private async Task ValidateReferencesAsync(
            int teacherId,
            int subjectId)
        {
            Teacher? teacher =
                await _teacherRepository
                    .GetByIdAsync(
                        teacherId
                    );


            if (teacher is null)
            {
                throw new ArgumentException(
                    "Обраного викладача не знайдено."
                );
            }


            Subject? subject =
                await _subjectRepository
                    .GetByIdAsync(
                        subjectId
                    );


            if (subject is null)
            {
                throw new ArgumentException(
                    "Обраний предмет не знайдено."
                );
            }
        }


        private static void ValidateSubgroup(
            int? subgroup)
        {
            if (!subgroup.HasValue)
            {
                return;
            }


            if (subgroup.Value is < 1 or > 255)
            {
                throw new ArgumentException(
                    "Номер підгрупи повинен бути від 1 до 255."
                );
            }
        }
    }
}