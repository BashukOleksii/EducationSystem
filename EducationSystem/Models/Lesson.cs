namespace EducationSystem.Models
{
    public sealed class Lesson
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public int TeacherSubjectId { get; set; }

        public int GroupId { get; set; }

        public DateTime Date { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}