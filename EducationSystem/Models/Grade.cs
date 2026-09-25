namespace EducationSystem.Models
{
    public sealed class Grade
    {
        public int Id { get; set; }
        public int LessonId { get; set; }

        public int StudentId { get; set; }

        public byte Value { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}