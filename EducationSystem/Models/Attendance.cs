using EducationSystem.Models.Enums;

namespace EducationSystem.Models
{
    public sealed class Attendance
    {
        public int Id { get; set; }

        public int LessonId { get; set; }

        public int StudentId { get; set; }

        public AttendanceStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}