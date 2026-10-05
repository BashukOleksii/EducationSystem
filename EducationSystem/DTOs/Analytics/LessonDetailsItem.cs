using EducationSystem.Models.Enums;

namespace EducationSystem.DTOs.Analytics
{
    public sealed class LessonDetailsItem
    {
        public int LessonId { get; set; }

        public string LessonTitle { get; set; }
            = string.Empty;

        public DateTime LessonDate { get; set; }


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

        public int PlannedSubjectHours { get; set; }


        public int GroupId { get; set; }

        public string GroupPrefix { get; set; }
            = string.Empty;

        public byte GroupNumber { get; set; }


        public byte? Subgroup { get; set; }


        public bool TeacherIsActive { get; set; }

        public bool SubjectIsActive { get; set; }

        public bool TeacherSubjectIsActive { get; set; }

        public bool GroupIsActive { get; set; }


        public string TeacherFullName =>
            $"{TeacherLastName} {TeacherFirstName}";


        public string GroupName =>
            $"{GroupPrefix}-{GroupNumber}";


        public string SubgroupText =>
            Subgroup.HasValue
                ? Subgroup.Value.ToString()
                : "—";
    }
}