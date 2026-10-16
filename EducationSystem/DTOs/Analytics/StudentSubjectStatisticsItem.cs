namespace EducationSystem.DTOs.Analytics
{
    public sealed class StudentSubjectStatisticsItem
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


        public int SubjectId { get; set; }

        public string SubjectName { get; set; }
            = string.Empty;


        public int LessonCount { get; set; }

        public int GradeCount { get; set; }

        public decimal? AverageGrade { get; set; }


        public int PresentCount { get; set; }

        public int MissingCount { get; set; }

        public int AttendanceMarkedCount { get; set; }

        public decimal? AttendancePercent { get; set; }


        public string StudentFullName =>
            $"{StudentLastName} {StudentFirstName}";


        public string GroupName =>
            $"{GroupPrefix}-{GroupNumber}";


        public int ConductedHours =>
            LessonCount * 2;
    }
}