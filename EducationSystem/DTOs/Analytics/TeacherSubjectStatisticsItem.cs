using EducationSystem.Models.Enums;

namespace EducationSystem.DTOs.Analytics
{
    public sealed class TeacherSubjectStatisticsItem
    {
        public int TeacherSubjectId { get; set; }


        public int TeacherId { get; set; }

        public string TeacherFirstName { get; set; }
            = string.Empty;

        public string TeacherLastName { get; set; }
            = string.Empty;

        public string TeacherEmail { get; set; }
            = string.Empty;

        public TeacherCategory TeacherCategory { get; set; }


        public int SubjectId { get; set; }

        public string SubjectName { get; set; }
            = string.Empty;


        public int PlannedHours { get; set; }


        public byte? Subgroup { get; set; }


        public int LessonCount { get; set; }

        public int ConductedHours { get; set; }

        public int GroupCount { get; set; }


        public DateTime? FirstLessonDate { get; set; }

        public DateTime? LastLessonDate { get; set; }


        public bool AssignmentIsActive { get; set; }

        public bool TeacherIsActive { get; set; }

        public bool SubjectIsActive { get; set; }


        public string TeacherFullName =>
            $"{TeacherLastName} {TeacherFirstName}";


        public string SubgroupText =>
            Subgroup.HasValue
                ? Subgroup.Value.ToString()
                : "—";
    }
}