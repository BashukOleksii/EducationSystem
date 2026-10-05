using EducationSystem.Models.Enums;

namespace EducationSystem.DTOs.Analytics
{
    public sealed class TeacherWorkloadItem
    {
        public int TeacherId { get; set; }


        public string TeacherFirstName { get; set; }
            = string.Empty;

        public string TeacherLastName { get; set; }
            = string.Empty;

        public string TeacherEmail { get; set; }
            = string.Empty;

        public TeacherCategory TeacherCategory { get; set; }


        public int ActiveSubjectCount { get; set; }

        public int ActivePlannedHours { get; set; }


        public int LessonCount { get; set; }

        public int ConductedHours { get; set; }

        public int GroupCount { get; set; }


        public DateTime? FirstLessonDate { get; set; }

        public DateTime? LastLessonDate { get; set; }


        public bool TeacherIsActive { get; set; }


        public string TeacherFullName =>
            $"{TeacherLastName} {TeacherFirstName}";
    }
}