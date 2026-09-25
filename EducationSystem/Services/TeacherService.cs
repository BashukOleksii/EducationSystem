using EducationSystem.DTOs.Common;
using EducationSystem.Models;
using EducationSystem.Models.Enums;
using EducationSystem.Repositories.Interfaces;
using System.Net.Mail;

namespace EducationSystem.Services
{
    public sealed class TeacherService
    {
        private readonly ITeacherRepository _teacherRepository;


        public TeacherService(
            ITeacherRepository teacherRepository)
        {
            _teacherRepository =
                teacherRepository;
        }


        public Task<PagedResult<Teacher>> GetPagedAsync(
            string? search,
            TeacherCategory? category,
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

            return _teacherRepository.GetPagedAsync(
                search,
                category,
                page,
                pageSize
            );
        }


        public Task<Teacher?> GetByIdAsync(
            int id)
        {
            return _teacherRepository
                .GetByIdAsync(id);
        }


        public async Task<int> CreateAsync(
            string firstName,
            string lastName,
            string email,
            TeacherCategory category)
        {
            Validate(
                firstName,
                lastName,
                email
            );


            string normalizedEmail =
                email
                    .Trim()
                    .ToLowerInvariant();


            bool emailExists =
                await _teacherRepository
                    .ExistsByEmailAsync(
                        normalizedEmail
                    );


            if (emailExists)
            {
                throw new ArgumentException(
                    "Викладач із такою електронною поштою вже існує."
                );
            }


            Teacher teacher =
                new Teacher
                {
                    FirstName =
                        firstName.Trim(),

                    LastName =
                        lastName.Trim(),

                    Email =
                        normalizedEmail,

                    Category =
                        category
                };


            return await _teacherRepository
                .CreateAsync(teacher);
        }


        public async Task UpdateAsync(
            int id,
            string firstName,
            string lastName,
            string email,
            TeacherCategory category)
        {
            Validate(
                firstName,
                lastName,
                email
            );


            string normalizedEmail =
                email
                    .Trim()
                    .ToLowerInvariant();


            bool emailExists =
                await _teacherRepository
                    .ExistsByEmailAsync(
                        normalizedEmail,
                        id
                    );


            if (emailExists)
            {
                throw new ArgumentException(
                    "Інший викладач із такою електронною поштою вже існує."
                );
            }


            Teacher teacher =
                new Teacher
                {
                    Id =
                        id,

                    FirstName =
                        firstName.Trim(),

                    LastName =
                        lastName.Trim(),

                    Email =
                        normalizedEmail,

                    Category =
                        category
                };


            await _teacherRepository
                .UpdateAsync(teacher);
        }


        public Task DeleteAsync(
            int id)
        {
            return _teacherRepository
                .DeleteAsync(id);
        }


        private static void Validate(
            string firstName,
            string lastName,
            string email)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException(
                    "Ім'я викладача не може бути порожнім."
                );
            }


            if (firstName.Trim().Length > 255)
            {
                throw new ArgumentException(
                    "Ім'я не може бути довшим за 255 символів."
                );
            }


            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException(
                    "Прізвище викладача не може бути порожнім."
                );
            }


            if (lastName.Trim().Length > 255)
            {
                throw new ArgumentException(
                    "Прізвище не може бути довшим за 255 символів."
                );
            }


            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException(
                    "Електронна пошта не може бути порожньою."
                );
            }


            if (email.Trim().Length > 255)
            {
                throw new ArgumentException(
                    "Електронна пошта занадто довга."
                );
            }


            if (!MailAddress.TryCreate(
                    email.Trim(),
                    out _))
            {
                throw new ArgumentException(
                    "Введено некоректну електронну пошту."
                );
            }
        }
    }
}