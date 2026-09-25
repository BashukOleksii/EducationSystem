namespace EducationSystem.Models
{
    public sealed class TeacherSubject
    {
        public int Id { get; set; }

        public int TeacherId { get; set; }

        public int SubjectId { get; set; }

        public byte? Subgroup { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}