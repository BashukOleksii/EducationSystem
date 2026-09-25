using EducationSystem.DTOs.Common;
using EducationSystem.Models;
using EducationSystem.Repositories.Interfaces;

namespace EducationSystem.Services
{
    public sealed class SubjectService
    {
        private readonly ISubjectRepository _subjectRepository;

        public SubjectService(
            ISubjectRepository subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }

        public Task<PagedResult<Subject>> GetPagedAsync(
            string? search,
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

            return _subjectRepository.GetPagedAsync(
                search,
                page,
                pageSize
            );
        }

        public Task<Subject?> GetByIdAsync(
            int id)
        {
            return _subjectRepository.GetByIdAsync(id);
        }

        public async Task<int> CreateAsync(
            string name,
            int duration)
        {
            Validate(
                name,
                duration
            );

            string normalizedName =
                name.Trim();

            bool exists =
                await _subjectRepository
                    .ExistsByNameAsync(
                        normalizedName
                    );

            if (exists)
            {
                throw new ArgumentException(
                    "Предмет із такою назвою вже існує."
                );
            }

            Subject subject =
                new Subject
                {
                    Name = normalizedName,
                    Duration = duration
                };

            return await _subjectRepository
                .CreateAsync(subject);
        }

        public async Task UpdateAsync(
            int id,
            string name,
            int duration)
        {
            Validate(
                name,
                duration
            );

            string normalizedName =
                name.Trim();

            bool exists =
                await _subjectRepository
                    .ExistsByNameAsync(
                        normalizedName,
                        id
                    );

            if (exists)
            {
                throw new ArgumentException(
                    "Інший предмет із такою назвою вже існує."
                );
            }

            Subject subject =
                new Subject
                {
                    Id = id,
                    Name = normalizedName,
                    Duration = duration
                };

            await _subjectRepository
                .UpdateAsync(subject);
        }

        public Task DeleteAsync(
            int id)
        {
            return _subjectRepository.DeleteAsync(id);
        }

        private static void Validate(
            string name,
            int duration)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "Назва предмета не може бути порожньою."
                );
            }

            if (name.Trim().Length > 255)
            {
                throw new ArgumentException(
                    "Назва предмета не може бути довшою за 255 символів."
                );
            }

            if (duration <= 0)
            {
                throw new ArgumentException(
                    "Тривалість предмета повинна бути більшою за 0."
                );
            }
        }
    }
}