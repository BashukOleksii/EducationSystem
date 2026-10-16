namespace EducationSystem.DTOs.Analytics
{
    public sealed class StudentLessonResultItem
    {
        public int StudentId { get; set; }

        public string StudentFirstName { get; set; }
            = string.Empty;

        public string StudentLastName { get; set; }
            = string.Empty;

        public string StudentEmail { get; set; }
            = string.Empty;


        public int GroupId { get; set; }

        public string GroupPrefix { get; set; }
            = string.Empty;

        public byte GroupNumber { get; set; }


        public int LessonId { get; set; }

        public string LessonTitle { get; set; }
            = string.Empty;

        public DateTime LessonDate { get; set; }


        public int SubjectId { get; set; }

        public string SubjectName { get; set; }
            = string.Empty;


        public int TeacherId { get; set; }

        public string TeacherFirstName { get; set; }
            = string.Empty;

        public string TeacherLastName { get; set; }
            = string.Empty;


        public byte? Subgroup { get; set; }


        public byte? Grade { get; set; }

        public string? AttendanceStatus { get; set; }


        public string StudentFullName =>
            $"{StudentLastName} {StudentFirstName}";


        public string TeacherFullName =>
            $"{TeacherLastName} {TeacherFirstName}";


        public string GroupName =>
            $"{GroupPrefix}-{GroupNumber}";


        public string GradeText =>
            Grade.HasValue
                ? Grade.Value.ToString()
                : "—";


        public string AttendanceStatusText =>
            AttendanceStatus switch
            {
                "present" => "Присутній",
                "missing" => "Відсутній",
                _ => "—"
            };
    }
}