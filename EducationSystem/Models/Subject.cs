namespace EducationSystem.Models
{
    public sealed class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Duration { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}