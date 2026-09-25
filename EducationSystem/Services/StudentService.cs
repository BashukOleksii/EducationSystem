using EducationSystem.DTOs.Common;
using EducationSystem.DTOs.Students;
using EducationSystem.Models;
using EducationSystem.Repositories.Interfaces;
using System.Net.Mail;

namespace EducationSystem.Services
{
    public sealed class StudentService
    {
        private readonly IStudentRepository _studentRepository;

        private readonly IGroupRepository _groupRepository;


        public StudentService(
            IStudentRepository studentRepository,
            IGroupRepository groupRepository)
        {
            _studentRepository =
                studentRepository;

            _groupRepository =
                groupRepository;
        }


        public Task<PagedResult<StudentListItem>> GetPagedAsync(
            string? search,
            int? groupId,
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


            return _studentRepository.GetPagedAsync(
                search,
                groupId,
                page,
                pageSize
            );
        }


        public Task<Student?> GetByIdAsync(
            int id)
        {
            return _studentRepository
                .GetByIdAsync(id);
        }


        public async Task<int> CreateAsync(
            string firstName,
            string lastName,
            string email,
            int groupId)
        {
            Validate(
                firstName,
                lastName,
                email,
                groupId
            );


            await ValidateGroupAsync(
                groupId
            );


            string normalizedEmail =
                email
                    .Trim()
                    .ToLowerInvariant();


            bool emailExists =
                await _studentRepository
                    .ExistsByEmailAsync(
                        normalizedEmail
                    );


            if (emailExists)
            {
                throw new ArgumentException(
                    "Студент із такою електронною поштою вже існує."
                );
            }


            Student student =
                new Student
                {
                    FirstName =
                        firstName.Trim(),

                    LastName =
                        lastName.Trim(),

                    Email =
                        normalizedEmail,

                    GroupId =
                        groupId
                };


            return await _studentRepository
                .CreateAsync(student);
        }


        public async Task UpdateAsync(
            int id,
            string firstName,
            string lastName,
            string email,
            int groupId)
        {
            Validate(
                firstName,
                lastName,
                email,
                groupId
            );


            await ValidateGroupAsync(
                groupId
            );


            string normalizedEmail =
                email
                    .Trim()
                    .ToLowerInvariant();


            bool emailExists =
                await _studentRepository
                    .ExistsByEmailAsync(
                        normalizedEmail,
                        id
                    );


            if (emailExists)
            {
                throw new ArgumentException(
                    "Інший студент із такою електронною поштою вже існує."
                );
            }


            Student student =
                new Student
                {
                    Id =
                        id,

                    FirstName =
                        firstName.Trim(),

                    LastName =
                        lastName.Trim(),

                    Email =
                        normalizedEmail,

                    GroupId =
                        groupId
                };


            await _studentRepository
                .UpdateAsync(student);
        }


        public Task DeleteAsync(
            int id)
        {
            return _studentRepository
                .DeleteAsync(id);
        }


        private async Task ValidateGroupAsync(
            int groupId)
        {
            Group? group =
                await _groupRepository
                    .GetByIdAsync(groupId);


            if (group is null)
            {
                throw new ArgumentException(
                    "Обрану групу не знайдено."
                );
            }
        }


        private static void Validate(
            string firstName,
            string lastName,
            string email,
            int groupId)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException(
                    "Ім'я студента не може бути порожнім."
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
                    "Прізвище студента не може бути порожнім."
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

            if (email.Trim().Length > 50)
            {
                throw new ArgumentException(
                    "Електронна пошта не може бути довшою за 50 символів."
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


            if (groupId <= 0)
            {
                throw new ArgumentException(
                    "Необхідно вибрати групу."
                );
            }
        }
    }
}