using EducationSystem.Models.Enums;

namespace EducationSystem.Models
{
    public sealed class Teacher
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public TeacherCategory Category { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string FullName =>
            $"{LastName} {FirstName}";
    }
}